using GlideNGlow.Socket.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace GlideNGlow.Socket.Wrappers.Installers;

public static class HubMapper
{
    public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<ConnectionHub>("/connection-hub");
        endpoints.MapHub<GameHub>("/game-hub");
        return endpoints;
    }
}