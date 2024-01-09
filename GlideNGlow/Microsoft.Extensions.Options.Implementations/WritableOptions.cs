using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Extensions.Options.Implementations;

public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
{
	private readonly IHostEnvironment _environment;
	private readonly IOptionsMonitor<T> _options;
	private readonly string _section;
	private readonly string _path;

	public WritableOptions(
		IHostEnvironment environment,
		IOptionsMonitor<T> options,
		string section,
		string path)
	{
		_environment = environment;
		_options = options;
		_section = section;
		_path = path;
	}

	public T CurrentValue => _options.CurrentValue;
	public T Get(string? name) => _options.Get(name);
	public IDisposable? OnChange(Action<T, string?> listener)
	{
		return _options.OnChange(listener);
	}

	public void Update(Action<T> applyChanges)
	{
#if DEBUG
		var fileProvider = _environment.ContentRootFileProvider;
		var fileInfo = fileProvider.GetFileInfo("appsettings.Development.json");
		var physicalPath = fileInfo.PhysicalPath;
#else
		var physicalPath = _path;
#endif

		if (physicalPath == null)
			throw new ArgumentNullException(nameof(physicalPath));

		var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));
		var sectionObject = jObject?.TryGetValue(_section, out var section) ?? throw new ArgumentNullException(nameof(jObject)) ?
			JsonConvert.DeserializeObject<T>(section.ToString()) :
			CurrentValue;

		applyChanges(sectionObject ?? new T());

		jObject[_section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
		File.WriteAllText(physicalPath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
	}
}