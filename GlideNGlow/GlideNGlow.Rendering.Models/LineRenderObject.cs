using System.Drawing;

namespace GlideNGlow.Rendering.Models;

public class LineRenderObject : RenderObject
{
    private int _baseOffset = 0;
    private int _direction = 0;
    private int _length = 0;
    private Color _color;
    List<Color> _image = new List<Color>();
    
    public LineRenderObject(int start, int vector, Color color)
    {
        if (vector > 0)
        {
            Offset = start;
            _direction = 1;
        }
        else if (vector < 0)
        {
            _direction = -1;
            Offset = start + vector;
            _baseOffset = -vector;
        }
        _length = int.Abs(vector);
        _color = color;
        Update();
    }
    
    public void SetBaseOffset(int offset)
    {
        _baseOffset = offset;
    }
    
    public override void SetX(int x)
    {
        Offset = x + _baseOffset;
    }
    
    public override void Update()
    {
        //fill from start to end
        _image.Clear();
        _image.Add(_color);
        if (_direction > 0)
        {
            for (int i = 0; i < _length; i++)
            {
                _image.Add(_color);
            }
        }
        else if (_direction < 0)
        {
            for (int i = _length; i > 0; i--)
            {
                _image.Add(_color);
            }
        }
    }
    
    
    public void SetColor(Color color)
    {
        _color = color;
    }
    
    
    public override List<Color> Image()
    {
        return _image;
    }
    
    
}