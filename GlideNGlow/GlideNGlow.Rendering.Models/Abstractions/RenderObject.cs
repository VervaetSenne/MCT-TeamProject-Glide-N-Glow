using GlideNGlow.Rendering.Handlers;

namespace GlideNGlow.Rendering.Models.Abstractions;

public abstract class RenderObject
{
    protected bool isDirty = true;
    public abstract void Render(LightRenderer lightRenderer);
}