using System.Drawing;

namespace GlideNGlow.Rendering.Models;

public class LineIdRenderObject : IdRenderObject
{
    private readonly int _direction;
    private readonly int _length;
    private readonly List<Color> _image = new();
    
    private int _baseOffset;
    private Color _color;
    
    public LineIdRenderObject(int start, int vector, Color color)
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
    
    public void Update()
    {
        //fill from start to end
        _image.Clear();
        _image.Add(_color);
        switch (_direction)
        {
            case > 0:
            {
                for (var i = 0; i < _length; i++)
                {
                    _image.Add(_color);
                }

                break;
            }
            case < 0:
            {
                for (var i = _length; i > 0; i--)
                {
                    _image.Add(_color);
                }

                break;
            }
        }
    }
    
    public void SetBaseOffset(int offset)
    {
        _baseOffset = offset;
    }
    
    public override void SetX(int x)
    {
        Offset = x + _baseOffset;
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