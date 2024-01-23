using System.Drawing;
using GlideNGlow.Rendering.Handlers;
using GlideNGlow.Rendering.Models.Abstractions;

namespace GlideNGlow.Rendering.Models;

public class MeasurementLineRenderObject : RenderObject
{
    private float _startPosition;
    private float _endPosition;
    private Color _color;
    
    public MeasurementLineRenderObject(float start, float end, Color color)
    {
        _startPosition = start;
        _endPosition = end;
        _color = color;
    }
    
    public void SetStart(float start)
    {
        _startPosition = start;
    }
    
    public void SetEnd(float end)
    {
        _endPosition = end;
    }
    
    public void Move(float x)
    {
        IsDirty = true;
        _startPosition += x;
        _endPosition += x;
    }
    
    //clamp color values between 0 and 128
    private static Color ClampColor(Color color, int clampValue = 128)
    {
        var ratio = clampValue / 255f;

        //remap values in color
        return Color.FromArgb((int)(color.R * ratio), (int)(color.G * ratio), (int)(color.B * ratio));
    }

    
    public void SetColor(Color color)
    {
        color = ClampColor(color);
        _color = color;
    }
    
    public void SetColor(int r, int g, int b)
    {
        SetColor(Color.FromArgb(r,g,b));
    }

    public override void Render(LightRenderer renderer)
    {
        if(IsDirty)
        {
            IsDirty = false;
            renderer.MakeDirty();
        }
        //first we must convert our start and end positions to the correct pixel positions
        if (!renderer.LightStripConverter.TryConvertToPixelLine(_startPosition, _endPosition, out var startPixel,
                out var endPixel)) return;
        
        if(startPixel == endPixel)
        {
            //we are on the same pixel, so we can just draw a point
            renderer.Lights[startPixel] = _color;
            return;
        }
        //make sure endPixel is reachable
        if (endPixel >= renderer.PixelAmount || endPixel < 0)
        {
            return;
        }
        
        //now we can draw the line
        var drawPosition = startPixel;
        do
        {
            if (renderer.Lights.Count <= drawPosition)
            {
                return;
            }
            renderer.Lights[drawPosition] = _color;
            drawPosition = (drawPosition + 1) % renderer.PixelAmount;
        } while (drawPosition != endPixel);
    }
}