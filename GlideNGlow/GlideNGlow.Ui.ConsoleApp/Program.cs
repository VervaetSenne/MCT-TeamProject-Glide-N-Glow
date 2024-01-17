using GlideNGlow.Mqqt.Models;
using GlideNGlow.Services.Installers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(config =>
    {
        var appsettings = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        var appsettingsDevelopment = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", true, true)
            .Build();

        config.AddConfiguration(appsettings);
        config.AddConfiguration(appsettingsDevelopment);
    })
    .ConfigureServices((context, services) =>
    {
        services
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<MqttHandler>()
            .AddSingleton<EspHandler>()
            .InstallServices(context.Configuration);
    }).Build();
    
using var scope = builder.Services.CreateScope();

var espHandler = scope.ServiceProvider.GetRequiredService<EspHandler>();
await espHandler.AddSubscriptions();

await builder.RunAsync();