using System.Drawing;

namespace GlideNGlow.Rendering.Models;

public class LerpIdRenderObject : IdRenderObject
{
    private int _direction = 0;
    private int _length = 0;
    private Color _colorStart;
    private Color _colorEnd;
    private List<Color> _image = new List<Color>();
    
    private int _baseOffset = 0;
    
    public LerpIdRenderObject(int start, int vector, Color colorStart, Color colorEnd)
    {
        if (vector > 0)
        {
            _direction = 1;
            Offset = start;
        }
        else if (vector < 0)
        {
            _direction = -1;
            Offset = start + vector;
            _baseOffset = -vector;
        }
        _length = int.Abs(vector);
        SetColor(colorStart,colorEnd);
    }

    public void Update()
    {
        //fill from start to end and lerp the colors
        _image.Clear();

        if (_direction > 0)
        {
            for (int i = 0; i < _length; i++)
            {
                _image.Add(Color.FromArgb(
                    (int)(_colorStart.R + (i * (_colorEnd.R - _colorStart.R) / (_length - 1))),
                    (int)(_colorStart.G + (i * (_colorEnd.G - _colorStart.G) / (_length - 1))),
                    (int)(_colorStart.B + (i * (_colorEnd.B - _colorStart.B) / (_length - 1)))
                ));
            }  
        }
        else if(_direction < 0)
        {
            for (int i =  _length; i > 0; i--)
            {
                _image.Add(Color.FromArgb(
                    (int)(_colorStart.R + (i * (_colorEnd.R - _colorStart.R) / (_length - 1))),
                    (int)(_colorStart.G + (i * (_colorEnd.G - _colorStart.G) / (_length - 1))),
                    (int)(_colorStart.B + (i * (_colorEnd.B - _colorStart.B) / (_length - 1)))
                ));
            }  
        }
    }
    
    public void SetColor(Color colorStart, Color colorEnd)
    {
        _colorStart = colorStart;
        _colorEnd = colorEnd;
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
    
    public override List<Color> Image()
    {
        return _image;
    }
    
}