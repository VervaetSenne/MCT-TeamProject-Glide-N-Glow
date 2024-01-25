namespace GlideNGlow.Socket.Abstractions;

public interface ISocketWrapper
{
    Task PublishUpdateGamemode(Guid? gameId);

    Task PublishUpdateScore(int playerIndex, string score);
}