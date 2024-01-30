using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options.Implementations;
using Microsoft.Extensions.Options.Implementations.Extensions;
using MQTTnet;
using MQTTnet.Client;

namespace GlideNGlow.Gamemodes.Handlers.Installers;

public static class EngineInstaller
{
    public static IServiceCollection InstallGamemodeEngine(this IServiceCollection services, IConfiguration config)
    {
        return services
            .ConfigureWritable<AppSettings>(config, nameof(AppSettings))
            .AddScoped<MqttHandler>()
            .AddScoped<LightButtonHandler>()
            .AddScoped<LightRenderer>(isp => LightRenderer.Create(isp.GetRequiredService<ILogger<LightRenderer>>(),
            isp.GetRequiredService<IWritableOptions<AppSettings>>(), isp.GetRequiredService<MqttHandler>()))
            .AddScoped<IMqttClient>(_ => new MqttFactory().CreateMqttClient())
            .AddScoped<GamemodeHandler>()
            .AddHostedService<Engine>();
    }
}