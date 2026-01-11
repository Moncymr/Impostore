namespace ImpostoreGame.Models;

public class Game
{
    public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
    public string Code { get; set; } = string.Empty;
    public string HostPlayerId { get; set; } = string.Empty;
    public GameState State { get; set; } = GameState.Lobby;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? SecretWord { get; set; }
    public string? ImpostorId { get; set; }
    public int CurrentTurnIndex { get; set; } = 0;
    public List<Player> Players { get; set; } = new();
    public List<Vote> Votes { get; set; } = new();
    public List<ChatMessage> Messages { get; set; } = new();
    public string? WinnerId { get; set; }
    public bool ImpostorsWon { get; set; }
}
