namespace GlideNGlow.Socket.Data;

public class ScoreHandler
{
    List<Score> _scores = new List<Score>();

    public ScoreHandler()
    {
        
    }

    public void AddScores(List<string> newScores)
    {
        for (var index = 0; index < newScores.Count; index++)
        {
            var score = newScores[index];
            _scores.Add(new Score(_scores.Count+index, score));
        }
    }

    public void ClaimScore(int scoreId, string playerName)
    {
        var score = _scores.FirstOrDefault(s => s.PlayerIndex == scoreId);
        if (score != null)
        {
            score.PlayerName = playerName;
        }
    }

    public void RemoveScores()
    {
        _scores.Clear();
    }
}
