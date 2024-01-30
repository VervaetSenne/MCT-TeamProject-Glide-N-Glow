using GlideNGlow.Socket.Hubs;

namespace GlideNGlow.Socket.Installers;

public static class HubMapper
{
    public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder builder)
    {
        builder.MapHub<ConnectionHub>("/connection-hub");
        builder.MapHub<GameHub>("/game-hub");
        return builder;
    }
}