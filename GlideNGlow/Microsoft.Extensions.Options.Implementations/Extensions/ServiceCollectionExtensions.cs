using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.Options.Implementations.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection ConfigureWritable<T>(this IServiceCollection services, IConfigurationBuilder config, string sectionName, string? path = null)
			where T : class, new()
		{
			path ??= config.GetFileProvider().GetDirectoryContents("").First(f => !f.IsDirectory).Name;
			if (path == null) throw new ArgumentNullException(nameof(path));

			var section = config.Build().GetSection(sectionName);
			services.Configure<T>(section);
			services.AddTransient<IWritableOptions<T>>(isp =>
			{
				var environment = isp.GetRequiredService<IHostEnvironment>();
				var options = isp.GetRequiredService<IOptionsMonitor<T>>();
				return new WritableOptions<T>(environment, options, section.Key, path);
			});
			return services;
		}
	}
}
