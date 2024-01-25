using Newtonsoft.Json;

namespace GlideNGlow.Core.Models.Extensions;

public static class SettingExtensions
{
    public static IEnumerable<Setting> GetSettingsObject(this Game game)
    {
        return JsonConvert.DeserializeObject<IEnumerable<Setting>>(game.Settings) ?? Enumerable.Empty<Setting>();
    }
}