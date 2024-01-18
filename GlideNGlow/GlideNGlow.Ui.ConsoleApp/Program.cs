using System.Drawing;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Gamemodes.Models;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;
using GlideNGlow.Services.Installers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options.Implementations;

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
            .AddSingleton<LightRenderer>()
            .AddSingleton<Engine>()
            .AddHostedService<Engine>()
            .InstallServices(context.Configuration);
    }).Build();
    
using var scope = builder.Services.CreateScope();

var appsettings = scope.ServiceProvider.GetRequiredService<IWritableOptions<AppSettings>>();
appsettings.Update(settings => settings.Ip = "10.10.10.252");
appsettings = scope.ServiceProvider.GetRequiredService<IWritableOptions<AppSettings>>();
var test = appsettings.CurrentValue.Ip;

var renderer = scope.ServiceProvider.GetRequiredService<LightRenderer>();

await builder.RunAsync();