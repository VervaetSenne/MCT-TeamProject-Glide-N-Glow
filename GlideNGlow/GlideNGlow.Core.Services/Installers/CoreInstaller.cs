using GlideNGlow.Core.Data;
using GlideNGlow.Core.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlideNGlow.Core.Services.Installers;

public static class CoreInstaller
{
    public static IServiceCollection InstallCore(this IServiceCollection services, IConfiguration config)
    {
        return services
            .AddDbContextFactory<GlideNGlowDbContext>(o =>
            {
#if DEBUG
                o.UseInMemoryDatabase("GlideNGlow");
#else
                var connectionString = config.GetConnectionString("default");
                o.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
#endif
            })
            .AddScoped<IEntryService, EntryService>()
            .AddScoped<IGameService, GameService>();
    }
}