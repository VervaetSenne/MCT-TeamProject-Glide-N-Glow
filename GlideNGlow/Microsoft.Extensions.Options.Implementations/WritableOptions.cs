using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options.Implementations.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Extensions.Options.Implementations;

public class WritableOptions<TOptions> : IWritableOptions<TOptions> where TOptions : class, new()
{
	private readonly IHostEnvironment _environment;
	private readonly IOptionsMonitor<TOptions> _options;
	private readonly string _section;

	public WritableOptions(
		IHostEnvironment environment,
		IOptionsMonitor<TOptions> options,
		string section)
	{
		_environment = environment;
		_options = options;
		_section = section;
	}

	public TOptions CurrentValue => _options.CurrentValue;
	public TOptions Get(string? name) => _options.Get(name);
	public IDisposable? OnChange(Action<TOptions, string?> listener)
	{
		return _options.OnChange(listener);
	}

	public void Update(Action<TOptions> applyChanges, bool doesWrite = true)
	{
#if DEBUG
		var fileProvider = _environment.ContentRootFileProvider;
		var fileInfo = fileProvider.GetFileInfo("appsettings.Development.json");
		var physicalPath = fileInfo.PhysicalPath;
#else
		var fileProvider = _environment.ContentRootFileProvider;
		var fileInfo = fileProvider.GetFileInfo("appsettings.json");
		var physicalPath = fileInfo.PhysicalPath;
#endif

		if (physicalPath == null)
			throw new ArgumentNullException(nameof(physicalPath));

		var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));
		var sectionObject =
			jObject?.TryGetValue(_section, out var section) ?? throw new ArgumentNullException(nameof(jObject))
				? JsonConvert.DeserializeObject<TOptions>(section.ToString())
				: CurrentValue;

		sectionObject ??= new TOptions();
		
		applyChanges(sectionObject);

		if (!doesWrite) return;
		
		jObject[_section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
		
		new MultiThreadFileWriter().WriteLine(new LogMessage
		{
			Filepath = physicalPath,
			Text = JsonConvert.SerializeObject(jObject, Formatting.Indented)
		});
	}
}