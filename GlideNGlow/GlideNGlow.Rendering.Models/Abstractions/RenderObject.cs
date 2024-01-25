using GlideNGlow.Rendering.Handlers;

namespace GlideNGlow.Rendering.Models.Abstractions;

public abstract class RenderObject
{
    protected bool IsVisible = true;
    protected bool IsDirty = true;
    public abstract void Render(LightRenderer lightRenderer);
    public abstract void SetVisibility(bool visibility);
}