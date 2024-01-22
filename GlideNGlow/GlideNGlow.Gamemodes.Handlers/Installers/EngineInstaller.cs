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
            .AddSingleton<MqttHandler>()
            .AddSingleton<EspHandler>()
            .AddSingleton<LightRenderer>(isp => LightRenderer.Create(isp.GetRequiredService<ILogger<LightRenderer>>(),
            isp.GetRequiredService<IOptionsMonitor<AppSettings>>(), isp.GetRequiredService<MqttHandler>()))
            .AddSingleton<IMqttClient>(_ => new MqttFactory().CreateMqttClient())
            .AddSingleton<Engine>();
    }
}