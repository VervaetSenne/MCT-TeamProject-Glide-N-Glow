using GlideNGlow.Socket.Abstractions;
using GlideNGlow.Socket.Constants;
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
    }

    public async Task PublishUpdateScore(int id, string score)
    {
        await _gamHubContext.Clients.All.SendAsync(Events.ScoreUpdated, id, score);
    }

    public async Task PublishNewScores(List<string> scores)
    {
        await _gamHubContext.Clients.All.SendAsync(Events.NewScores, scores);
        _scoreHandler.AddScores(scores);
    }

    public async Task PublishScoreClaimedAsync(int id, string playerName)
    {
        await _gamHubContext.Clients.All.SendAsync(Events.ScoreUpdated, id, playerName);
    }
}