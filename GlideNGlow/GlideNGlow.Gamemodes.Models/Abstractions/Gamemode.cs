using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Models.Abstractions;

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