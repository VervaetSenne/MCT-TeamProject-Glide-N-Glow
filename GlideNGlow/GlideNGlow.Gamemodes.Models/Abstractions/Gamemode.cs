using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Models.Abstractions;
using Newtonsoft.Json;

namespace GlideNGlow.Gamemodes.Models.Abstractions;

public abstract class Gamemode : IGamemode
{
    protected readonly List<RenderObject> RenderObjects = new();
    
    protected readonly EspHandler EspHandler;
    protected readonly AppSettings AppSettings;
    protected bool ForceRenderUpdate = true;

    protected Gamemode(EspHandler espHandler, AppSettings appSettings)
    {
        EspHandler = espHandler;
        AppSettings = appSettings;
    }

    public abstract void Initialize();

    public abstract void Stop();

    public abstract Task UpdateAsync(TimeSpan timeSpan);

    public virtual List<RenderObject> GetRenderObjects()
    {
        return RenderObjects;
    }

    public abstract Task ButtonPressed(int id);

    public bool ShouldForceRender()
    {
        if (ForceRenderUpdate)
        {
            ForceRenderUpdate= false;
            return true;
        }

        return false;
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