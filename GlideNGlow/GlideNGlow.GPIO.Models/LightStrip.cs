using GlideNGlow.Common.Models.Settings;
using Iot.Device.Ws28xx;
using Microsoft.Extensions.Options;

namespace GlideNGlow.GPIO.Models;

//entire light circle, so multiple light strips
public class LightStrip
{
    private List<LightPixel> Pixels = new();
    
    public LightStrip(IOptionsMonitor<AppSettings> appSettings)
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