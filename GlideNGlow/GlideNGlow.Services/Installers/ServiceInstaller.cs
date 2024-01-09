using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Core.Services.Installers;
using GlideNGlow.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options.Implementations.Extensions;

namespace GlideNGlow.Services.Installers;

public static class ServiceInstaller
{
    public static IServiceCollection InstallServices(this IServiceCollection services, IConfiguration config)
    {
        return services
            .InstallCore(config)
            .ConfigureWritable<AppSettings>(config, nameof(AppSettings))
            .AddScoped<IAdminService, AdminService>()
            .AddScoped<IUserService, UserService>();
    }
}
