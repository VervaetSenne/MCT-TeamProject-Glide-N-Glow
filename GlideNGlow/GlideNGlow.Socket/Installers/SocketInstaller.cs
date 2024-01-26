using GlideNGlow.Socket.Abstractions;
using GlideNGlow.Socket.Data;

namespace GlideNGlow.Socket.Installers;

public static class SocketInstaller
{
    public static IServiceCollection InstallSockets(this IServiceCollection services)
    {
        services
            .AddSignalR();
        return services
            .AddScoped<ISocketWrapper, SocketWrapper>()
            .AddSingleton<ScoreHandler>();
    }
}