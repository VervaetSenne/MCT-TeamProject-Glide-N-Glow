using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GlideNGlow.Gamemodes.Handlers;

public class Engine : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public Engine(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = StartInBackGround(cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StartInBackGround(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var gamemodeHandler = scope.ServiceProvider.GetRequiredService<GamemodeHandler>();
        
        var deltaTime = TimeSpan.FromMilliseconds(300);
        var periodicTimer = new PeriodicTimer(deltaTime);
        while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            await gamemodeHandler.TryInitializeAsync(cancellationToken);
            await gamemodeHandler.UpdateAsync(deltaTime);
            await gamemodeHandler.RenderAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}