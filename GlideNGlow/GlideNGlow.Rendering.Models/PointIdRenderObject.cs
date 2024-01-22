/*using System.Drawing;
using GlideNGlow.Rendering.Models.Abstractions;

namespace GlideNGlow.Rendering.Models;

public class PointRenderObject : RenderObject
{
    private Color _color;
    
    public PointRenderObject(int x, Color color)
    {
        Offset = x;
        _color = color;
    }
    
    public void SetColor(Color color)
    {
        _color = color;
    }
    
    public override List<Color> Image()
    {
        //return dictionary with the x and color, 1 object
        return new List<Color>
        {
            _color
        };
    }
}*/