using GlideNGlow.Mqqt.Handlers;
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

    private async Task StartInBackGround(CancellationToken cancellationToken)
    {
        Initialize();
        
        using var scope = _scopeFactory.CreateScope();
        var gamemodeHandler = scope.ServiceProvider.GetRequiredService<GamemodeHandler>();
        
        var deltaTime = TimeSpan.FromMilliseconds(300);
        var periodicTimer = new PeriodicTimer(deltaTime);
        while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            await gamemodeHandler.TryInitializeAsync(cancellationToken);
            await gamemodeHandler.UpdateAsync(deltaTime, cancellationToken);
            await gamemodeHandler.RenderAsync(cancellationToken);
        }
    }

    private void Initialize()
    {
        using var scope = _scopeFactory.CreateScope();
        var lightButtonHandler = scope.ServiceProvider.GetRequiredService<LightButtonHandler>();
        
        lightButtonHandler.OnStartupFileChange();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}