using System.Drawing;

namespace GlideNGlow.Rendering.Models;

public class PointIdRenderObject : IdRenderObject
{
    private Color _color;
    
    public PointIdRenderObject(int x, Color color)
    {
        Offset = x;
        this._color = color;
    }
    
    public void SetColor(Color color)
    {
        this._color = color;
    }
    
    public override List<Color> Image()
    {
        //return dictionary with the x and color, 1 object
        return new List<Color>()
        {
            _color
        };
    }
    
}