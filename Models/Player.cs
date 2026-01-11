namespace ImpostoreGame.Models;

public class Player
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Nickname { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public bool IsHost { get; set; }
    public bool IsApproved { get; set; }
    public bool IsImpostor { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
    public bool IsConnected { get; set; } = true;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
