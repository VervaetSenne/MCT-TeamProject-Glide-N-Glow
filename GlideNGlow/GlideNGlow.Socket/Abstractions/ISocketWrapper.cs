namespace GlideNGlow.Socket.Abstractions;

public interface ISocketWrapper
{
    Task PublishUpdateGamemode(Guid? gameId);
    
    Task PublishUpdateScore(int id, string score);
    Task PublishNewScores(params string[] scores);
    Task PublishScoreClaimedAsync(int id, string playerName);

    Task SendButtonsUpdated();
    
    Task ButtonConnected(string macAddress, float distanceFromStart);

    Task ButtonDisconnected(string macAddress);
    
    Task SendWarning(string message);
}