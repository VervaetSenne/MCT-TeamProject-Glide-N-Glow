using GlideNGlow.Socket.Data;

namespace GlideNGlow.Socket;

public class ScoreHandler
{
    public Guid GameId { get; private set; }
    
    public readonly List<Score> Scores = new();

    public void AddScores(List<string> newScores)
    {
        for (var index = 0; index < newScores.Count; index++)
        {
            var score = newScores[index];
            Scores.Add(new Score(Scores.Count + index, score));
        }
    }

    public Score? ClaimScore(int scoreId, string playerName)
    {
        if (Scores.Count <= scoreId)
            return null;
        
        Scores[scoreId].PlayerName = playerName;

        return Scores[scoreId];
    }

    public void RemoveScores(Guid newGameId)
    {
        GameId = newGameId;
        Scores.Clear();
    }
}
