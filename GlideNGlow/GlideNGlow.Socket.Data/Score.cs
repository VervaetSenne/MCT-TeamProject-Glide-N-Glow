namespace GlideNGlow.Socket.Data;

public class Score
{
    public int PlayerIndex { get; set; }
    public string Value { get; set; }
    public string PlayerName { get; set; } = "";
    
    public Score(int playerIndex, string value)
    {
        PlayerIndex = playerIndex;
        Value = value;
    }
}