using Microsoft.AspNetCore.SignalR;
using ImpostoreGame.Services;
using ImpostoreGame.Models;

namespace ImpostoreGame.Hubs;

public class GameHub : Hub
{
    private readonly GameService _gameService;

    public GameHub(GameService gameService)
    {
        _gameService = gameService;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGameGroup(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        
        // Notify all players in the group about the updated game state
        // This ensures that when a player joins an already-started game,
        // other players receive the connection update
        var game = await _gameService.GetGameByIdAsync(gameId);
        if (game != null && game.State != GameState.Lobby)
        {
            await Clients.Group(gameId).SendAsync("GameUpdated", game);
        }
    }
    
    public async Task NotifyPlayerJoined(string gameId, string playerId, string nickname)
    {
        var game = await _gameService.GetGameByIdAsync(gameId);
        if (game == null)
            return;
            
        var player = game.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null)
            return;
        
        // Notify the host about the new player join request
        await NotifyHostAboutPlayerJoin(game, gameId, player);
        
        // Send updated game state to ALL players so everyone sees the complete player list
        await Clients.Group(gameId).SendAsync("GameUpdated", game);
    }

    public async Task LeaveGameGroup(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
    }

    public async Task RequestJoinGame(string gameCode, string playerId, string nickname)
    {
        var game = await _gameService.GetGameByCodeAsync(gameCode);
        if (game == null)
        {
            await Clients.Caller.SendAsync("JoinRequestFailed", "Partita non trovata");
            return;
        }

        var (player, isNewPlayer) = await _gameService.AddPlayerToGameAsync(game.Id, playerId, nickname);
        if (player == null)
        {
            await Clients.Caller.SendAsync("JoinRequestFailed", "Impossibile unirsi alla partita");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
        
        // Notify the host about the new player join request (only if truly new)
        if (isNewPlayer)
        {
            await NotifyHostAboutPlayerJoin(game, game.Id, player);
            
            // Refresh game state to include the newly added player and send to all players
            var updatedGame = await _gameService.GetGameByIdAsync(game.Id);
            if (updatedGame != null)
            {
                await Clients.Group(game.Id).SendAsync("GameUpdated", updatedGame);
            }
        }
        
        await Clients.Caller.SendAsync("JoinRequestSent", game.Id);
    }

    private async Task NotifyHostAboutPlayerJoin(Game game, string gameId, Player player)
    {
        // Find the host player
        var host = game.Players.FirstOrDefault(p => p.IsHost);
        if (host == null)
            return;
            
        // Notify all players in the game group except the caller about the new player join request
        // The client-side will handle showing approval UI only to the host
        await Clients.OthersInGroup(gameId).SendAsync("PlayerJoinRequest", player);
    }

    public async Task ApprovePlayer(string gameId, string playerId)
    {
        var success = await _gameService.ApprovePlayerAsync(gameId, playerId);
        if (success)
        {
            var game = await _gameService.GetGameByIdAsync(gameId);
            await Clients.Group(gameId).SendAsync("PlayerApproved", playerId);
            await Clients.Group(gameId).SendAsync("GameUpdated", game);
        }
    }

    public async Task RejectPlayer(string gameId, string playerId)
    {
        var success = await _gameService.RejectPlayerAsync(gameId, playerId);
        if (success)
        {
            await Clients.Group(gameId).SendAsync("PlayerRejected", playerId);
        }
    }

    public async Task StartGame(string gameId)
    {
        var success = await _gameService.StartGameAsync(gameId);
        if (success)
        {
            var game = await _gameService.GetGameByIdAsync(gameId);
            // Explicitly send to caller (host) first to ensure they receive the event
            await Clients.Caller.SendAsync("GameStarted", game);
            // Then send to others in the group
            await Clients.OthersInGroup(gameId).SendAsync("GameStarted", game);
            
            // Add and broadcast system message
            var startMessage = await _gameService.AddSystemMessageAsync(gameId, "La partita è iniziata!");
            if (startMessage != null)
            {
                await Clients.Group(gameId).SendAsync("ReceiveMessage", startMessage);
            }
            
            // Notify all players about whose turn it is
            if (game != null)
            {
                var approvedPlayers = game.Players.Where(p => p.IsApproved).ToList();
                var currentTurnPlayer = approvedPlayers.ElementAtOrDefault(game.CurrentTurnIndex);
                if (currentTurnPlayer != null)
                {
                    await Clients.Group(gameId).SendAsync("TurnChanged", currentTurnPlayer.Id, currentTurnPlayer.Nickname);
                    
                    // Add and broadcast turn notification message
                    var turnMessage = await _gameService.AddSystemMessageAsync(gameId, $"È il turno di {currentTurnPlayer.Nickname}!");
                    if (turnMessage != null)
                    {
                        await Clients.Group(gameId).SendAsync("ReceiveMessage", turnMessage);
                    }
                }
            }
        }
    }

    public async Task SendMessage(string gameId, string playerId, string message)
    {
        var success = await _gameService.AddMessageAsync(gameId, playerId, message);
        if (success)
        {
            var game = await _gameService.GetGameByIdAsync(gameId);
            var chatMessage = game?.Messages.LastOrDefault();
            if (chatMessage != null)
            {
                await Clients.Group(gameId).SendAsync("ReceiveMessage", chatMessage);
            }
            
            // Auto-advance turn if in turn-based phase (InProgress)
            if (game?.State == GameState.InProgress)
            {
                // Check if the message sender is the current turn player
                var approvedPlayers = game.Players.Where(p => p.IsApproved).ToList();
                var currentTurnPlayer = approvedPlayers.ElementAtOrDefault(game.CurrentTurnIndex);
                
                if (currentTurnPlayer?.Id == playerId)
                {
                    // Automatically advance to next turn
                    await NextTurn(gameId);
                }
            }
        }
    }

    public async Task NextTurn(string gameId)
    {
        var success = await _gameService.MoveToNextTurnAsync(gameId);
        if (success)
        {
            var game = await _gameService.GetGameByIdAsync(gameId);
            
            if (game?.State == GameState.InProgress)
            {
                // Check if all turns are complete
                var approvedPlayers = game.Players.Where(p => p.IsApproved).ToList();
                if (game.CurrentTurnIndex >= approvedPlayers.Count)
                {
                    // All players have had their turn and everyone is ready
                    var message = await _gameService.AddSystemMessageAsync(gameId, "Tutti pronti! La votazione inizierà presto...");
                    if (message != null)
                    {
                        await Clients.Group(gameId).SendAsync("ReceiveMessage", message);
                    }
                }
                else
                {
                    // Check if we're restarting the turn cycle
                    var currentTurnPlayer = approvedPlayers.ElementAtOrDefault(game.CurrentTurnIndex);
                    if (currentTurnPlayer != null)
                    {
                        // Check if this is the first player and we're cycling
                        if (game.CurrentTurnIndex == 0)
                        {
                            var allHadTurn = approvedPlayers.Any(p => p.IsReadyToVote);
                            if (allHadTurn)
                            {
                                var cycleMessage = await _gameService.AddSystemMessageAsync(gameId, "Nuovo giro di turni! Non tutti sono pronti a votare.");
                                if (cycleMessage != null)
                                {
                                    await Clients.Group(gameId).SendAsync("ReceiveMessage", cycleMessage);
                                }
                            }
                        }
                        
                        // Notify about turn change
                        await Clients.Group(gameId).SendAsync("TurnChanged", currentTurnPlayer.Id, currentTurnPlayer.Nickname);
                        
                        // Add and broadcast turn notification message
                        var turnMessage = await _gameService.AddSystemMessageAsync(gameId, $"È il turno di {currentTurnPlayer.Nickname}!");
                        if (turnMessage != null)
                        {
                            await Clients.Group(gameId).SendAsync("ReceiveMessage", turnMessage);
                        }
                    }
                }
            }
            
            // Send updated game state after all messages have been added
            var updatedGame = await _gameService.GetGameByIdAsync(gameId);
            await Clients.Group(gameId).SendAsync("GameUpdated", updatedGame);
        }
    }

    public async Task StartVoting(string gameId)
    {
        var success = await _gameService.StartVotingAsync(gameId);
        if (success)
        {
            var game = await _gameService.GetGameByIdAsync(gameId);
            await Clients.Group(gameId).SendAsync("VotingStarted", game);
            
            // Add and broadcast system message
            var message = await _gameService.AddSystemMessageAsync(gameId, "Fase di votazione iniziata!");
            if (message != null)
            {
                await Clients.Group(gameId).SendAsync("ReceiveMessage", message);
            }
        }
    }

    public async Task CastVote(string gameId, string voterId, string targetPlayerId)
    {
        var success = await _gameService.CastVoteAsync(gameId, voterId, targetPlayerId);
        if (success)
        {
            await Clients.Group(gameId).SendAsync("VoteCast", voterId);
            
            var game = await _gameService.GetGameByIdAsync(gameId);
            var approvedPlayerCount = game?.Players.Count(p => p.IsApproved) ?? 0;
            var voteCount = game?.Votes.Count ?? 0;
            
            if (voteCount >= approvedPlayerCount)
            {
                await FinishGame(gameId);
            }
        }
    }

    public async Task FinishGame(string gameId)
    {
        var success = await _gameService.FinishGameAsync(gameId);
        if (success)
        {
            var game = await _gameService.GetGameByIdAsync(gameId);
            await Clients.Group(gameId).SendAsync("GameFinished", game);
            
            var winner = game?.Players.FirstOrDefault(p => p.Id == game.WinnerId);
            var impostor = game?.Players.FirstOrDefault(p => p.Id == game.ImpostorId);
            
            string resultMessage;
            if (game?.ImpostorsWon == true)
            {
                resultMessage = $"L'impostore {impostor?.Nickname ?? "sconosciuto"} ha vinto! La parola era: {game.SecretWord ?? "sconosciuta"}";
            }
            else
            {
                resultMessage = $"I giocatori hanno vinto! L'impostore era {impostor?.Nickname ?? "sconosciuto"}. La parola era: {game.SecretWord ?? "sconosciuta"}";
            }
            
            // Add and broadcast system message
            var message = await _gameService.AddSystemMessageAsync(gameId, resultMessage);
            if (message != null)
            {
                await Clients.Group(gameId).SendAsync("ReceiveMessage", message);
            }
        }
    }

    public async Task SetPlayerReadyToVote(string gameId, string playerId)
    {
        var success = await _gameService.SetPlayerReadyToVoteAsync(gameId, playerId, true);
        if (success)
        {
            var game = await _gameService.GetGameByIdAsync(gameId);
            await Clients.Group(gameId).SendAsync("PlayerReadyUpdated", playerId);
            
            // Check if all players are ready
            var approvedPlayers = game?.Players.Where(p => p.IsApproved).ToList() ?? new List<Player>();
            var readyCount = approvedPlayers.Count(p => p.IsReadyToVote);
            
            if (readyCount == approvedPlayers.Count && approvedPlayers.Count > 0)
            {
                // All players are ready, start voting phase
                await StartVoting(gameId);
            }
            else
            {
                await Clients.Group(gameId).SendAsync("GameUpdated", game);
            }
        }
    }

    public async Task CastVoteByName(string gameId, string voterId, string targetPlayerName)
    {
        var success = await _gameService.CastVoteByNameAsync(gameId, voterId, targetPlayerName);
        if (success)
        {
            await Clients.Group(gameId).SendAsync("VoteCast", voterId);
            
            var game = await _gameService.GetGameByIdAsync(gameId);
            var approvedPlayerCount = game?.Players.Count(p => p.IsApproved) ?? 0;
            var voteCount = game?.Votes.Count ?? 0;
            
            if (voteCount >= approvedPlayerCount)
            {
                await FinishGame(gameId);
            }
        }
    }

    public async Task StartRematch(string gameId)
    {
        var success = await _gameService.ResetGameForRematchAsync(gameId);
        if (success)
        {
            // Automatically start the new game instead of going back to lobby
            var startSuccess = await _gameService.StartGameAsync(gameId);
            if (startSuccess)
            {
                var game = await _gameService.GetGameByIdAsync(gameId);
                await Clients.Group(gameId).SendAsync("RematchStarted", game);
                
                // Add and broadcast system message
                var startMessage = await _gameService.AddSystemMessageAsync(gameId, "Nuova partita iniziata!");
                if (startMessage != null)
                {
                    await Clients.Group(gameId).SendAsync("ReceiveMessage", startMessage);
                }
                
                // Notify all players about whose turn it is
                if (game != null)
                {
                    var approvedPlayers = game.Players.Where(p => p.IsApproved).ToList();
                    var currentTurnPlayer = approvedPlayers.ElementAtOrDefault(game.CurrentTurnIndex);
                    if (currentTurnPlayer != null)
                    {
                        await Clients.Group(gameId).SendAsync("TurnChanged", currentTurnPlayer.Id, currentTurnPlayer.Nickname);
                        
                        // Add and broadcast turn notification message
                        var turnMessage = await _gameService.AddSystemMessageAsync(gameId, $"È il turno di {currentTurnPlayer.Nickname}!");
                        if (turnMessage != null)
                        {
                            await Clients.Group(gameId).SendAsync("ReceiveMessage", turnMessage);
                        }
                    }
                }
            }
        }
    }

    public async Task UpdatePlayerConnection(string playerId, string gameId, bool isConnected)
    {
        await _gameService.UpdatePlayerConnectionAsync(playerId, Context.ConnectionId, isConnected);
        await Clients.Group(gameId).SendAsync("PlayerConnectionUpdated", playerId, isConnected);
    }
}
