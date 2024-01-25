using GlideNGlow.Socket.Wrappers.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace GlideNGlow.Socket.Wrappers.Installers;

public static class SocketInstaller
{
    public static IServiceCollection InstallSockets(this IServiceCollection services)
    {
        services
            .AddSignalR();
        return services
            .AddScoped<ISocketWrapper, SocketWrapper>();
    }
}