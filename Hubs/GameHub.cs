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

        var player = await _gameService.AddPlayerToGameAsync(game.Id, playerId, nickname);
        if (player == null)
        {
            await Clients.Caller.SendAsync("JoinRequestFailed", "Impossibile unirsi alla partita");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
        
        // Notify the host about the new player join request
        await NotifyHostAboutPlayerJoin(game, game.Id, player);
        
        await Clients.Caller.SendAsync("JoinRequestSent", game.Id);
    }

    private async Task NotifyHostAboutPlayerJoin(Game game, string gameId, Player player)
    {
        // Find the host player
        var host = game.Players.FirstOrDefault(p => p.IsHost);
        if (host == null)
            return;
            
        // Notify only the host about the new player join request
        if (!string.IsNullOrEmpty(host.ConnectionId))
        {
            await Clients.Client(host.ConnectionId).SendAsync("PlayerJoinRequest", player);
        }
        // Note: If host ConnectionId is not set, we skip the notification
        // The host will see the player when they refresh or when the player is approved
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
            await Clients.Group(gameId).SendAsync("GameStarted", game);
            await _gameService.AddSystemMessageAsync(gameId, "La partita è iniziata!");
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
        }
    }

    public async Task NextTurn(string gameId)
    {
        var success = await _gameService.MoveToNextTurnAsync(gameId);
        if (success)
        {
            var game = await _gameService.GetGameByIdAsync(gameId);
            await Clients.Group(gameId).SendAsync("GameUpdated", game);
            
            if (game?.State == GameState.Discussion)
            {
                await _gameService.AddSystemMessageAsync(gameId, "Fase di discussione iniziata!");
                await Clients.Group(gameId).SendAsync("DiscussionStarted");
            }
        }
    }

    public async Task StartVoting(string gameId)
    {
        var success = await _gameService.StartVotingAsync(gameId);
        if (success)
        {
            var game = await _gameService.GetGameByIdAsync(gameId);
            await Clients.Group(gameId).SendAsync("VotingStarted", game);
            await _gameService.AddSystemMessageAsync(gameId, "Fase di votazione iniziata!");
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
            
            if (game?.ImpostorsWon == true)
            {
                await _gameService.AddSystemMessageAsync(gameId, $"L'impostore {impostor?.Nickname ?? "sconosciuto"} ha vinto! La parola era: {game.SecretWord ?? "sconosciuta"}");
            }
            else
            {
                await _gameService.AddSystemMessageAsync(gameId, $"I giocatori hanno vinto! L'impostore era {impostor?.Nickname ?? "sconosciuto"}. La parola era: {game.SecretWord ?? "sconosciuta"}");
            }
        }
    }

    public async Task UpdatePlayerConnection(string playerId, string gameId, bool isConnected)
    {
        await _gameService.UpdatePlayerConnectionAsync(playerId, Context.ConnectionId, isConnected);
        await Clients.Group(gameId).SendAsync("PlayerConnectionUpdated", playerId, isConnected);
    }
}
