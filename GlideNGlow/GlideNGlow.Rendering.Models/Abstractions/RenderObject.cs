using System.Drawing;
using GlideNGlow.Rendering.Handlers;

namespace GlideNGlow.Rendering.Models.Abstractions;

public abstract class RenderObject
{
    public abstract void Render(LightRenderer lightRenderer);
}