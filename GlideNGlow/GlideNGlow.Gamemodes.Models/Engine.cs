using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GlideNGlow.Gamemodes.Models;

public class Engine : IHostedService
{
    private readonly LightRenderer _lightRenderer;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly EspHandler _espHandler;
    
    public Engine(LightRenderer lightRenderer, IOptionsMonitor<AppSettings> appSettings, EspHandler espHandler)
    {
        _lightRenderer = lightRenderer;
        _appSettings = appSettings;
        _espHandler = espHandler;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _espHandler.AddSubscriptions(cancellationToken);
        var gamemodeHandler = new GamemodeHandler(_lightRenderer, _appSettings, _espHandler);
        
        gamemodeHandler.Start();
        
        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(0.3));
        while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            await gamemodeHandler.UpdateAsync(0.3f);
            await gamemodeHandler.RenderAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}