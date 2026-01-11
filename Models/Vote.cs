namespace ImpostoreGame.Models;

public class Vote
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string GameId { get; set; } = string.Empty;
    public string VoterId { get; set; } = string.Empty;
    public string TargetPlayerId { get; set; } = string.Empty;
    public DateTime VotedAt { get; set; } = DateTime.UtcNow;
}
