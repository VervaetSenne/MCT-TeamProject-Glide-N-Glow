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
        _startPosition += x;
        _endPosition += x;
    }
    
    public void SetColor(Color color)
    {
        _color = color;
    }
    
    public void SetColor(int r, int g, int b)
    {
        _color = Color.FromArgb(r,g,b);
    }

    public override void Render(LightRenderer renderer)
    {
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