namespace ImpostoreGame.Models;

public class Word
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;
}
