using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Models;

namespace GlideNGlow.Gamemodes.Models.Abstractions;

public abstract class Gamemode
{
    protected EspHandler EspHandler;
    protected AppSettings AppSettings;

    protected Gamemode(EspHandler espHandler, AppSettings appSettings)
    {
        EspHandler = espHandler;
        AppSettings = appSettings;
    }
}