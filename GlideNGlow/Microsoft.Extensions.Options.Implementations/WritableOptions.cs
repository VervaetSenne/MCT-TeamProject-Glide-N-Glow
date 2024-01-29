using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Extensions.Options.Implementations;

public class WritableOptions<TOptions> : IWritableOptions<TOptions> where TOptions : class, new()
{
	private readonly IHostEnvironment _environment;
	private readonly IOptionsMonitor<TOptions> _options;
	private readonly string _section;

	private readonly BlockingCollection<Actions> _actionsCollection = new();

	public WritableOptions(
		IHostEnvironment environment,
		IOptionsMonitor<TOptions> options,
		string section)
	{
		_environment = environment;
		_options = options;
		_section = section;

		_ = StartConsumptionAsync();
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
		
		_actionsCollection.Add(new Actions
		{
			Obj = sectionObject,
			Action = applyChanges,
			DoesWrite = doesWrite,
			PhysicalPath = physicalPath,
			JObject = jObject
		});
	}

	private Task StartConsumptionAsync()
	{
		foreach (var action in _actionsCollection.GetConsumingEnumerable())
		{
			action.Action(action.Obj);

			if (!action.DoesWrite) continue;
		
			action.JObject[_section] = JObject.Parse(JsonConvert.SerializeObject(action.Obj));
			try
			{
				File.WriteAllText(action.PhysicalPath, JsonConvert.SerializeObject(action.JObject, Formatting.Indented));
			}
			catch
			{
				Task.Delay(2).GetAwaiter().GetResult();
				File.WriteAllText(action.PhysicalPath, JsonConvert.SerializeObject(action.JObject, Formatting.Indented));
			}
		}

		return Task.CompletedTask;
	}

	private class Actions
	{
		public required TOptions Obj { get; init; }
		public required Action<TOptions> Action { get; init; }
		public required bool DoesWrite { get; init; }
		public required string PhysicalPath { get; init; }
		public required JObject JObject { get; init; }
	}
}