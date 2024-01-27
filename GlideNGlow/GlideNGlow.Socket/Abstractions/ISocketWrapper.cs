namespace GlideNGlow.Socket.Abstractions;

public interface ISocketWrapper
{
    Task PublishUpdateGamemode(Guid? gameId);
    
    Task PublishUpdateScore(int id, string score);
    Task PublishNewScores(List<string> scores);
    Task PublishScoreClaimedAsync(int id, string playerName);
}