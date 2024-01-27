using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Models.Abstractions;
using GlideNGlow.Socket.Abstractions;
using Newtonsoft.Json;

namespace GlideNGlow.Gamemodes.Models.Abstractions;

public abstract class Gamemode : IGamemode
{
    protected readonly List<RenderObject> RenderObjects = new();
    
    protected readonly LightButtonHandler LightButtonHandler;
    protected readonly AppSettings AppSettings;
    protected readonly ISocketWrapper SocketWrapper;
    protected bool ForceRenderUpdate = true;
    
    //private IGamemode _gamemodeImplementation;

    protected Gamemode(LightButtonHandler lightButtonHandler, AppSettings appSettings, ISocketWrapper socketWrapper)
    {
        LightButtonHandler = lightButtonHandler;
        AppSettings = appSettings;
        SocketWrapper = socketWrapper;
    }
    
    public abstract void Initialize(CancellationToken cancellationToken);

    public abstract void Stop();

    public abstract Task UpdateAsync(TimeSpan timeSpan, CancellationToken cancellationToken);

    public virtual List<RenderObject> GetRenderObjects()
    {
        return RenderObjects.ToList();
    }

    public abstract Task ButtonPressed(int id, CancellationToken cancellationToken);

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
    protected readonly TSettings Settings;

    protected Gamemode(LightButtonHandler lightButtonHandler, AppSettings appSettings,  ISocketWrapper socketWrapper, string settingsJson) : base(lightButtonHandler, appSettings,socketWrapper)
    {
        Settings = JsonConvert.DeserializeObject<TSettings>(settingsJson)
                   ?? throw new ArgumentNullException(nameof(settingsJson), "Settings given to gamemode do not conform to model!");
    }
}