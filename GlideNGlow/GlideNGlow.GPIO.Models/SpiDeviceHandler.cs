using System.Device.Spi;
using System.Drawing;
using Iot.Device.Ws28xx;
using GlideNGlow.Common.Models.Settings;
using Microsoft.Extensions.Options;

namespace GlideNGlow.GPIO.Models;

public class SpiDeviceHandler : IDisposable
{
    SpiConnectionSettings _settings;
    private readonly SpiDevice spiDevice;
    private Ws2812b _ws2812B;
    private readonly RawPixelContainer _image;


    public SpiDeviceHandler(IOptionsMonitor<AppSettings> appSettings)
    {
        _settings = new SpiConnectionSettings(0,0)
        {
            ClockFrequency = 2_400_000,
            Mode = SpiMode.Mode0,
            DataBitLength = 8
        };

        spiDevice = SpiDevice.Create(_settings);
        
        _ws2812B = new Ws2812b(spiDevice, appSettings.CurrentValue.Strips.Aggregate(0, (i, strip) => i + strip.Leds));

    }
    
    public void SetPixel(int pixelId, byte r, byte g, byte b)
    {
        Color color = Color.FromArgb(r, g, b);
        var img = _ws2812B.Image;
        img.SetPixel(pixelId, 0, color);
    }
    
    public void SetPixel(int pixelId, Color color)
    {
        var img = _ws2812B.Image;
        img.SetPixel(pixelId, 0, color);
    }
    
    public void LightUpPixel(int pixelId,byte r, byte g, byte b)
    {
        Color color = Color.FromArgb(r, g, b);
        var img = _ws2812B.Image;
        img.SetPixel(pixelId, 0, color);
        _ws2812B.Update();
    }
    
    public void LightUpPixel(int pixelId,Color color)
    {
        var img = _ws2812B.Image;
        img.SetPixel(pixelId, 0, color);
        _ws2812B.Update();
    }
    
    public void LightOutPixel(int pixelId)
    {
        var img = _ws2812B.Image;
        img.SetPixel(pixelId, 0, Color.Black);
        _ws2812B.Update();
    }

    public void LightUpRange(int startId, int stopId, byte r, byte g, byte b)
    {
        var img = _ws2812B.Image;
        for (int i = startId; i < stopId; i++)
        {
            img.SetPixel(i, 0, Color.FromArgb(r, g, b));
        }
        _ws2812B.Update();
    }
    
    public void LightUpRange(int startId, int stopId, Color color)
    {
        var img = _ws2812B.Image;
        for (int i = startId; i < stopId; i++)
        {
            img.SetPixel(i, 0, color);
        }
        _ws2812B.Update();
    }
    
    public void LightOutRange(int startId, int stopId)
    {
        var img = _ws2812B.Image;
        for (int i = startId; i < stopId; i++)
        {
            img.SetPixel(i, 0, Color.Black);
        }
        _ws2812B.Update();
    }
    
    public void LightUpLerp(int startId, int stopId, Color startColor, Color stopColor)
    {
        var img = _ws2812B.Image;
        for (int i = startId; i < stopId; i++)
        {
            img.SetPixel(i, 0, Color.FromArgb(
                (int) Math.Round(startColor.R + (stopColor.R - startColor.R) * ((double) i / (stopId - startId))),
                (int) Math.Round(startColor.G + (stopColor.G - startColor.G) * ((double) i / (stopId - startId))),
                (int) Math.Round(startColor.B + (stopColor.B - startColor.B) * ((double) i / (stopId - startId)))
            ));
        }
        _ws2812B.Update();
    }
    
    

    public void Dispose()
    {
        spiDevice.Dispose();
    }
}