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
                var connectionString = config.GetConnectionString("default");
                o.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            })
            .AddScoped<IEntryService, EntryService>()
            .AddScoped<IGameService, GameService>();
    }
}