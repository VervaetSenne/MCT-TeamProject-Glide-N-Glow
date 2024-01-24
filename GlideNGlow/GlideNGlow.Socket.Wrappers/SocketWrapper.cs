using GlideNGlow.Socket.Hubs;
using GlideNGlow.Socket.Wrappers.Abstractions;
using GlideNGlow.Socket.Wrappers.Constants;
using Microsoft.AspNetCore.SignalR;

namespace GlideNGlow.Socket.Wrappers;

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
}