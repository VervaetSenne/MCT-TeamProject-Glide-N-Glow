namespace GlideNGlow.Socket.Wrappers.Abstractions;

public interface ISocketWrapper
{
    Task PublishUpdateGamemode(Guid? gameId);
}