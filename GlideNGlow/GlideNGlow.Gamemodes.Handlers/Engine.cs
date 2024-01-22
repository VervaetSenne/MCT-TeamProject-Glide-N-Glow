using GlideNGlow.Mqqt.Handlers;
using Microsoft.Extensions.Hosting;

namespace GlideNGlow.Gamemodes.Handlers;

public class Engine : IHostedService
{
    private readonly GamemodeHandler _gamemodeHandler;
    
    public Engine(GamemodeHandler gamemodeHandler)
    {
        _gamemodeHandler = gamemodeHandler;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var deltaTime = TimeSpan.FromMilliseconds(300);
        var periodicTimer = new PeriodicTimer(deltaTime);
        while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            await _gamemodeHandler.TryInitializeAsync(cancellationToken);
            await _gamemodeHandler.UpdateAsync(deltaTime);
            await _gamemodeHandler.RenderAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}