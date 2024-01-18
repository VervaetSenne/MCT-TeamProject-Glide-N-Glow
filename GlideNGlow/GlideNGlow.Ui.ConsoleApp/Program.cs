using System.Drawing;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Gamemodes.Models;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;
using GlideNGlow.Services.Installers;
// using Iot.Device.Nmea0183.Sentences;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options.Implementations;

var builder = Host.CreateDefaultBuilder()
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
