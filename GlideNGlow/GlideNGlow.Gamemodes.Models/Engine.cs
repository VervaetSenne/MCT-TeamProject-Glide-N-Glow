using Microsoft.Extensions.Hosting;

namespace GlideNGlow.Gamemodes.Models;

public class Engine : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(0.3));
        while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

//periodic timer opzoeken