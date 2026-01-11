using ImpostoreGame.Models;
using ImpostoreGame.Data;
using Microsoft.EntityFrameworkCore;

namespace ImpostoreGame.Services;

public class GameService
{
    private readonly GameDbContext _context;
    private readonly WordService _wordService;

    public GameService(GameDbContext context, WordService wordService)
    {
        _context = context;
        _wordService = wordService;
    }

    public async Task<Game> CreateGameAsync(string hostPlayerId, string hostNickname)
    {
        var game = new Game
        {
            Code = GenerateGameCode(),
            HostPlayerId = hostPlayerId,
            State = GameState.Lobby
        };

        var hostPlayer = new Player
        {
            Id = hostPlayerId,
            Nickname = hostNickname,
            GameId = game.Id,
            IsHost = true,
            IsApproved = true
        };

        game.Players.Add(hostPlayer);
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return game;
    }

    public async Task<Game?> GetGameByCodeAsync(string code)
    {
        return await _context.Games
            .Include(g => g.Players)
            .Include(g => g.Messages)
            .Include(g => g.Votes)
            .FirstOrDefaultAsync(g => g.Code == code);
    }

    public async Task<Game?> GetGameByIdAsync(string gameId)
    {
        return await _context.Games
            .Include(g => g.Players)
            .Include(g => g.Messages)
            .Include(g => g.Votes)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    public async Task<Player?> AddPlayerToGameAsync(string gameId, string playerId, string nickname)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null || game.State != GameState.Lobby)
            return null;

        // Check if player already exists in this game
        var existingPlayer = game.Players.FirstOrDefault(p => p.Id == playerId);
        if (existingPlayer != null)
            return existingPlayer;

        var player = new Player
        {
            Id = playerId,
            Nickname = nickname,
            GameId = gameId,
            IsApproved = false
        };

        game.Players.Add(player);
        await _context.SaveChangesAsync();

        return player;
    }

    public async Task<bool> ApprovePlayerAsync(string gameId, string playerId)
    {
        var player = await _context.Players
            .FirstOrDefaultAsync(p => p.Id == playerId && p.GameId == gameId);

        if (player == null)
            return false;

        player.IsApproved = true;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RejectPlayerAsync(string gameId, string playerId)
    {
        var player = await _context.Players
            .FirstOrDefaultAsync(p => p.Id == playerId && p.GameId == gameId);

        if (player == null)
            return false;

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> StartGameAsync(string gameId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null || game.State != GameState.Lobby)
            return false;

        var approvedPlayers = game.Players.Where(p => p.IsApproved).ToList();
        if (approvedPlayers.Count < 3)
            return false;

        // Assign impostor
        var impostorIndex = Random.Shared.Next(approvedPlayers.Count);
        var impostor = approvedPlayers[impostorIndex];
        impostor.IsImpostor = true;
        game.ImpostorId = impostor.Id;

        // Assign secret word and hint
        var word = await _wordService.GetRandomWordAsync();
        if (word != null)
        {
            game.SecretWord = word.Text;
            game.WordHint = word.Hint;
        }
        else
        {
            game.SecretWord = "Parola";
            game.WordHint = "Categoria generica";
        }
        
        game.State = GameState.InProgress;
        game.CurrentTurnIndex = 0;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AddMessageAsync(string gameId, string playerId, string message)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
            return false;

        var player = game.Players.FirstOrDefault(p => p.Id == playerId);
        if (player == null)
            return false;

        var chatMessage = new ChatMessage
        {
            GameId = gameId,
            PlayerId = playerId,
            PlayerName = player.Nickname,
            Message = message
        };

        game.Messages.Add(chatMessage);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AddSystemMessageAsync(string gameId, string message)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
            return false;

        var chatMessage = new ChatMessage
        {
            GameId = gameId,
            PlayerId = "system",
            PlayerName = "Sistema",
            Message = message,
            IsSystemMessage = true
        };

        game.Messages.Add(chatMessage);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MoveToNextTurnAsync(string gameId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null || game.State != GameState.InProgress)
            return false;

        var approvedPlayers = game.Players.Where(p => p.IsApproved).ToList();
        game.CurrentTurnIndex++;

        if (game.CurrentTurnIndex >= approvedPlayers.Count)
        {
            game.State = GameState.Discussion;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> StartVotingAsync(string gameId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
            return false;

        game.State = GameState.Voting;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CastVoteAsync(string gameId, string voterId, string targetPlayerId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null || game.State != GameState.Voting)
            return false;

        // Check if already voted
        var existingVote = game.Votes.FirstOrDefault(v => v.VoterId == voterId);
        if (existingVote != null)
            return false;

        var vote = new Vote
        {
            GameId = gameId,
            VoterId = voterId,
            TargetPlayerId = targetPlayerId
        };

        game.Votes.Add(vote);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> FinishGameAsync(string gameId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null || game.State != GameState.Voting)
            return false;

        var approvedPlayers = game.Players.Where(p => p.IsApproved).ToList();
        
        // Count votes
        var voteCounts = game.Votes
            .GroupBy(v => v.TargetPlayerId)
            .Select(g => new { PlayerId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        if (voteCounts != null)
        {
            game.WinnerId = voteCounts.PlayerId;
            
            // Check if impostor was caught
            if (game.ImpostorId == voteCounts.PlayerId)
            {
                game.ImpostorsWon = false;
            }
            else
            {
                game.ImpostorsWon = true;
            }
        }
        else
        {
            // No votes or tie - impostor wins
            game.ImpostorsWon = true;
        }

        game.State = GameState.Finished;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SetPlayerReadyToVoteAsync(string gameId, string playerId, bool isReady)
    {
        var player = await _context.Players
            .FirstOrDefaultAsync(p => p.Id == playerId && p.GameId == gameId);

        if (player == null)
            return false;

        player.IsReadyToVote = isReady;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CastVoteByNameAsync(string gameId, string voterId, string targetPlayerName)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null || game.State != GameState.Voting)
            return false;

        // Check if already voted
        var existingVote = game.Votes.FirstOrDefault(v => v.VoterId == voterId);
        if (existingVote != null)
            return false;

        // Find the target player by name (case-insensitive match)
        var targetPlayer = game.Players
            .FirstOrDefault(p => p.IsApproved && 
                p.Nickname.Equals(targetPlayerName.Trim(), StringComparison.OrdinalIgnoreCase));

        var vote = new Vote
        {
            GameId = gameId,
            VoterId = voterId,
            TargetPlayerId = targetPlayer?.Id ?? string.Empty,
            TargetPlayerName = targetPlayerName.Trim()
        };

        game.Votes.Add(vote);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ResetGameForRematchAsync(string gameId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null || game.State != GameState.Finished)
            return false;

        // Reset game state
        game.State = GameState.Lobby;
        game.SecretWord = null;
        game.WordHint = null;
        game.ImpostorId = null;
        game.CurrentTurnIndex = 0;
        game.WinnerId = null;
        game.ImpostorsWon = false;

        // Reset player states but keep them in the game
        foreach (var player in game.Players)
        {
            player.IsImpostor = false;
            player.IsReadyToVote = false;
            // Keep IsApproved, IsHost, ConnectionId as they are
        }

        // Clear votes and messages
        var votesToRemove = game.Votes.ToList();
        var messagesToRemove = game.Messages.ToList();
        
        foreach (var vote in votesToRemove)
        {
            _context.Votes.Remove(vote);
        }
        
        foreach (var message in messagesToRemove)
        {
            _context.ChatMessages.Remove(message);
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task UpdatePlayerConnectionAsync(string playerId, string connectionId, bool isConnected)
    {
        var player = await _context.Players.FirstOrDefaultAsync(p => p.Id == playerId);
        if (player != null)
        {
            player.ConnectionId = connectionId;
            player.IsConnected = isConnected;
            await _context.SaveChangesAsync();
        }
    }

    private string GenerateGameCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }
}
