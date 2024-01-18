using System.Drawing;

namespace GlideNGlow.Rendering.Models;

public abstract class IdRenderObject : RenderObject
{
    public abstract List<Color> Image();
    
    //overrideable function SetX with base implementation
    public virtual void SetX(int x)
    {
        Offset = x;
    }
    
    public virtual void Move(int x)
    {
        Offset += x;
    }
    
    public int GetOffset()
    {
        return Offset;
    }

    protected int Offset;
}