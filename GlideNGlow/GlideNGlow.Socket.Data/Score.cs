namespace GlideNGlow.Socket.Data;

public class Score
{
    public int PlayerIndex { get; set; }
    public string ScoreValue { get; set; }

    public string PlayerName { get; set; } = "";
    
    public Score(int playerIndex, string scoreValue)
    {
        PlayerIndex = playerIndex;
        ScoreValue = scoreValue;
    }
    
}