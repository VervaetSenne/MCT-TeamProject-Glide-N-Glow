using GlideNGlow.Socket.Abstractions;
using GlideNGlow.Socket.Constants;
using GlideNGlow.Socket.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GlideNGlow.Socket;

public class SocketWrapper : ISocketWrapper
{
    private readonly IHubContext<ConnectionHub> _connectionHubContext;
    private readonly IHubContext<GameHub> _gamHubContext;

    public SocketWrapper(IHubContext<ConnectionHub> connectionHubContext, IHubContext<GameHub> gamHubContext)
    {
        _connectionHubContext = connectionHubContext;
        _gamHubContext = gamHubContext;
    }

    public async Task PublishUpdateGamemode(Guid? gameId)
    {
        await _connectionHubContext.Clients.All.SendAsync(Events.CurrentGameUpdated, gameId);
    }

    public async Task PublishUpdateScore(int playerIndex, string score)
    {
        await _gamHubContext.Clients.All.SendAsync(Events.ScoreUpdated, playerIndex, score);
    }
}