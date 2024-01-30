using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.Options.Implementations.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection ConfigureWritable<T>(this IServiceCollection services, IConfiguration config, string sectionName)
			where T : class, new()
		{
			var section = config.GetSection(sectionName);
			services.Configure<T>(section);
			services.AddTransient<IWritableOptions<T>>(isp =>
			{
				var environment = isp.GetRequiredService<IHostEnvironment>();
				var options = isp.GetRequiredService<IOptionsMonitor<T>>();
				return new WritableOptions<T>(environment, options, section.Key);
			});
			return services;
		}
	}
}
