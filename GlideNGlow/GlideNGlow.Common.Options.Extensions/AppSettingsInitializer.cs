using GlideNGlow.Common.Models.Settings;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GlideNGlow.Common.Options.Extensions;

public static class AppSettingsInitializer
{
    public static void InitializeAppSettings(this IHostEnvironment environment)
    {
        var filePath = Path.Combine(environment.ContentRootPath, "appsettings.json");
        if (!File.Exists(filePath))
        {
            File.Create(Path.Combine(environment.ContentRootPath, "appsettings.json"));
            var text = new
            {
                ConnectionString = new
                {
                    @default = ""
                },
                AppSettings = new AppSettings()
            };
            File.WriteAllText(filePath, JsonConvert.SerializeObject(text));
            throw new Exception("Please fill in 'ip' and 'connectionstring' in the appsettings.json file!");
        }

        var settings = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath))?.Value<AppSettings>(nameof(AppSettings)) ?? throw new Exception();
        settings.CurrentSettings = "";
        settings.CurrentGamemode = null;
        File.WriteAllText(filePath, JsonConvert.SerializeObject(settings));
    }
}