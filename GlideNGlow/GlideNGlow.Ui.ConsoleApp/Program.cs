using GlideNGlow.Mqqt.Models;
using GlideNGlow.Core.Services.Installers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton(provider =>
                new MqttHandler("10.10.10.13", provider.GetRequiredService<ILogger<MqttHandler>>()))
            .AddSingleton(provider => new EspHandler("10.10.10.13",
                provider.GetRequiredService<ILogger<MqttHandler>>(), provider.GetRequiredService<MqttHandler>()))
            .InstallCore(context.Configuration);
    }).Build();
    
using var scope = builder.Services.CreateScope();

var espHandler = scope.ServiceProvider.GetRequiredService<EspHandler>();
await espHandler.AddSubscriptions();

await builder.RunAsync();