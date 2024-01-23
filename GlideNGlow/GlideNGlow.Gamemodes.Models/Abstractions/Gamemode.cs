using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Models.Abstractions;
using Newtonsoft.Json;

namespace GlideNGlow.Gamemodes.Models.Abstractions;

public abstract class Gamemode
{
    protected readonly List<RenderObject> RenderObjects = new();
    
    protected EspHandler EspHandler;
    protected AppSettings AppSettings;

    protected Gamemode(EspHandler espHandler, AppSettings appSettings)
    {
        EspHandler = espHandler;
        AppSettings = appSettings;
    }
}

public abstract class Gamemode<TSettings> : Gamemode
{
    protected TSettings Settings;

    protected Gamemode(EspHandler espHandler, AppSettings appSettings, string settingsJson) : base(espHandler, appSettings)
    {
        Settings = JsonConvert.DeserializeObject<TSettings>(settingsJson)
                   ?? throw new ArgumentNullException(nameof(settingsJson), "Settings given to gamemode do not conform to model!");
    }
}