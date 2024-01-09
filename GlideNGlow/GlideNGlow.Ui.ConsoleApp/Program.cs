using GlideNGlow.Core.Services.Installers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.InstallCore(context.Configuration);
    });
    
builder.RunConsoleAsync();