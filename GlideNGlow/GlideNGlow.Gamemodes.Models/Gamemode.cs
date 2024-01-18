using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;

namespace GlideNGlow.Gamemodes.Models;

public abstract class Gamemode
{
    protected EspHandler _espHandler;
    
    public abstract Task Start();
    public abstract Task Update(float deltaSeconds);
    public abstract List<RenderObject> GetRenderObjects();

    public abstract Task Input(int id);
}