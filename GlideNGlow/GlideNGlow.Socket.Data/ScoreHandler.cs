namespace GlideNGlow.Socket.Data;

public class ScoreHandler
{
    public readonly List<Score> Scores = new List<Score>();

    public ScoreHandler()
    {
        
    }

    public void AddScores(List<string> newScores)
    {
        for (var index = 0; index < newScores.Count; index++)
        {
            var score = newScores[index];
            Scores.Add(new Score(Scores.Count+index, score));
        }
    }

    public void ClaimScore(int scoreId, string playerName)
    {
        var score = Scores.FirstOrDefault(s => s.PlayerIndex == scoreId);
        if (score != null)
        {
            score.PlayerName = playerName;
        }
    }

    public void RemoveScores()
    {
        Scores.Clear();
    }
}
