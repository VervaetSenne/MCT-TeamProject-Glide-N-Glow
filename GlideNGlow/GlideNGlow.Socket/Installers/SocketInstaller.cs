using GlideNGlow.Socket.Abstractions;

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