using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GlideNGlow.Gamemodes.Models;

public class Engine : IHostedService
{
    GamemodeHandler _gamemodeHandler;
    public Engine(LightRenderer lightRenderer, IOptionsMonitor<AppSettings> appSettings, EspHandler espHandler)
    {
        _gamemodeHandler = new GamemodeHandler(lightRenderer, appSettings, espHandler);
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(0.3));
        while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            await _gamemodeHandler.Update(0.3f);
            await _gamemodeHandler.Render();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}