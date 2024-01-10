using Iot.Device.Ws28xx;

namespace GlideNGlow.GPIO.Models;

public class LightStrip
{
    private List<LightPixel> Pixels = new();
    
    public LightStrip()
    {
        
    }

    public LightStrip(int pixelAmount)
    {
        for (int i = 0; i < pixelAmount; i++)
        {
            Pixels.Add(new LightPixel());
        }
    }
    
    public void LightUpPixel(int pixelId)
    {
        
    }

    public void LightUpPixel(int pixelId,byte r, byte g, byte b)
    {
        
    }
    
    
}