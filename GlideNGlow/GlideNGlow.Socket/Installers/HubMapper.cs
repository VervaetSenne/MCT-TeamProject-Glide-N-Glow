using GlideNGlow.Socket.Hubs;

namespace GlideNGlow.Socket.Installers;

public static class HubMapper
{
    public static IApplicationBuilder MapHubs(this IApplicationBuilder app)
    {
        return app.UseEndpoints(builder =>
        {
            builder.MapHub<ConnectionHub>("/connection-hub");
            builder.MapHub<GameHub>("/game-hub");
        });
    }
}