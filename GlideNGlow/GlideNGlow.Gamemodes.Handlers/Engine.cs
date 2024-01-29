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
        Initialize();
        _ = StartInBackGround(cancellationToken);
        _ = StartLightButtonConnectionProberInBackground(cancellationToken);
        return Task.CompletedTask;
    }

    private void Initialize()
    {
        using var scope = _scopeFactory.CreateScope();
        var lightButtonHandler = scope.ServiceProvider.GetRequiredService<LightButtonHandler>();
        
        lightButtonHandler.OnStartupFileChange();
    }

    private async Task StartInBackGround(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var gamemodeHandler = scope.ServiceProvider.GetRequiredService<GamemodeHandler>();
        
        var deltaTime = TimeSpan.FromMilliseconds(300);
        var periodicTimer = new PeriodicTimer(deltaTime);
        await gamemodeHandler.InitButtonSubscriptionsAsync(cancellationToken);
        while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            await gamemodeHandler.TryInitializeAsync(cancellationToken);
            await gamemodeHandler.UpdateAsync(deltaTime, cancellationToken);
            await gamemodeHandler.RenderAsync(cancellationToken);
        }
    }

    private async Task StartLightButtonConnectionProberInBackground(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var lightButtonHandler = scope.ServiceProvider.GetRequiredService<LightButtonHandler>();

        var periodicTimer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            await lightButtonHandler.TestConnectionsAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var gamemodeHandler = scope.ServiceProvider.GetRequiredService<GamemodeHandler>();
        await gamemodeHandler.RemoveButtonSubscriptionsAsync(cancellationToken);
    }
}