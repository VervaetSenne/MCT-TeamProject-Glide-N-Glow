using System.Drawing;

namespace GlideNGlow.Rendering.Models;

public class PointRenderObject : RenderObject
{
    private Color _color;
    
    public PointRenderObject(int x, Color color)
    {
        Offset = x;
        this._color = color;
    }
    
    public override void Update()
    {
        //do nothing
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