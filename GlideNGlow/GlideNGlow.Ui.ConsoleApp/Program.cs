using GlideNGlow.GPIO.Models;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Services.Installers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<MqttHandler>()
            .AddSingleton<EspHandler>()
            .AddSingleton<SpiDeviceHandler>()
            .InstallServices(context.Configuration);
    }).Build();
    
using var scope = builder.Services.CreateScope();

var espHandler = scope.ServiceProvider.GetRequiredService<EspHandler>();
await espHandler.AddSubscriptions();
var spiDeviceHandler = scope.ServiceProvider.GetRequiredService<SpiDeviceHandler>();
_= spiDeviceHandler.TestAsync();

await builder.RunAsync();