using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;

namespace GlideNGlow.Gamemodes.Models;

public abstract class Gamemode
{
    private readonly EspHandler _espHandler;
    public abstract Task Update();
    public abstract List<RenderObject> GetRenderObjects();
    
}