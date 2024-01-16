﻿using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Core.Services.Installers;
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
            .ConfigureWritable<AppSettings>(config, nameof(AppSettings));
    }
}
