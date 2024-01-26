using GlideNGlow.Socket.Abstractions;
using GlideNGlow.Socket.Constants;
using GlideNGlow.Socket.Data;
using GlideNGlow.Socket.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GlideNGlow.Socket;

public class SocketWrapper : ISocketWrapper
{
    private readonly IHubContext<ConnectionHub> _connectionHubContext;
    private readonly IHubContext<GameHub> _gamHubContext;
    private readonly ScoreHandler _scoreHandler;

    public SocketWrapper(IHubContext<ConnectionHub> connectionHubContext, IHubContext<GameHub> gamHubContext, ScoreHandler scoreHandler)
    {
        _connectionHubContext = connectionHubContext;
        _gamHubContext = gamHubContext;
        _scoreHandler = scoreHandler;
    }

    public async Task PublishUpdateGamemode(Guid? gameId)
    {
        await _connectionHubContext.Clients.All.SendAsync(Events.CurrentGameUpdated, gameId);
        _scoreHandler.RemoveScores();
    }

    public async Task PublishUpdateScore(int playerIndex, string score)
    {
        await _gamHubContext.Clients.All.SendAsync(Events.ScoreUpdated, playerIndex, score);
    }

    public async Task PublishNewScores(List<string> scores)
    {
        await _gamHubContext.Clients.All.SendAsync(Events.NewScores, scores);
        _scoreHandler.AddScores(scores);
    }
}