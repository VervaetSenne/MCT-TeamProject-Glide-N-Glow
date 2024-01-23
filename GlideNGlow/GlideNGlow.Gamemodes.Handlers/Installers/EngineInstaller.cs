using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;

namespace GlideNGlow.Gamemodes.Handlers.Installers;

public static class EngineInstaller
{
    public static IServiceCollection InstallGamemodeEngine(this IServiceCollection services)
    {
        return services
            .AddScoped<MqttHandler>()
            .AddScoped<EspHandler>()
            .AddScoped<LightRenderer>(isp => LightRenderer.Create(isp.GetRequiredService<ILogger<LightRenderer>>(),
            isp.GetRequiredService<IOptionsMonitor<AppSettings>>(), isp.GetRequiredService<MqttHandler>()))
            .AddScoped<IMqttClient>(_ => new MqttFactory().CreateMqttClient())
            .AddScoped<GamemodeHandler>()
            .AddHostedService<Engine>();
    }
}