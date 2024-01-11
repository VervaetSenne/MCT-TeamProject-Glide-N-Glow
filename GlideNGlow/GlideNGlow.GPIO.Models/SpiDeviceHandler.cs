using System.Device.Spi;
using System.Drawing;
using Iot.Device.Ws28xx;
using GlideNGlow.Common.Models.Settings;
using Microsoft.Extensions.Options;

namespace GlideNGlow.GPIO.Models;

public class SpiDeviceHandler : IDisposable
{
    private readonly SpiDevice _spiDevice;
    private Ws2812b _ws2812B;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private int _pixelAmount;

    public SpiDeviceHandler(IOptionsMonitor<AppSettings> appSettings)
    {
        _spiDevice = SpiDevice.Create(new SpiConnectionSettings(0,0)
        {
            ClockFrequency = 2_400_000,
            Mode = SpiMode.Mode0,
            DataBitLength = 8
        });
        
        _appSettings = appSettings;
        _pixelAmount = GetCurrentAppSettings().Strips.Aggregate(0, (i, strip) => i + strip.Leds);
        
        _ws2812B = new Ws2812b(_spiDevice, _pixelAmount);
    }
    
    private void UpdateSettings()
    {
        _pixelAmount = GetCurrentAppSettings().Strips.Aggregate(0, (i, strip) => i + strip.Leds);
        
        _ws2812B = new Ws2812b(_spiDevice, _pixelAmount);
    }
    
    private AppSettings GetCurrentAppSettings()
    {
        return _appSettings.CurrentValue;
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
    
    public void LightUpRange(int startId, int vector, Color color)
    {
        var img = _ws2812B.Image;
        
        var direction = (vector - startId) / Math.Abs(vector - startId);
        var length = Math.Abs(vector - startId);

        if (direction < 0)
        {
            for (int i = 0; i < length; i++)
            {
                int pixelId = startId + i * direction;
                if (pixelId < 0)
                {
                    pixelId = _pixelAmount + pixelId;
                }
                img.SetPixel(pixelId, 0, color);
            } 
        }
        else
        {
            for (int i = 0; i < length; i++)
            {
                int pixelId = startId + i * direction;
                if (pixelId > _pixelAmount)
                {
                    pixelId = pixelId - _pixelAmount;
                }

                img.SetPixel(pixelId, 0, color);
            } 
        }
        _ws2812B.Update();
    }
    
    public void LightOutRange(int startId, int vector)
    {
        var img = _ws2812B.Image;
        LightUpRange(startId, vector, Color.Black);
        
        var direction = (vector - startId) / Math.Abs(vector - startId);
        var length = Math.Abs(vector - startId);

        if (direction < 0)
        {
            for (int i = 0; i < length; i++)
            {
                int pixelId = startId + i * direction;
                if (pixelId < 0)
                {
                    pixelId = _pixelAmount + pixelId;
                }
                img.SetPixel(pixelId, 0, Color.Black);
            } 
        }
        else
        {
            for (int i = 0; i < length; i++)
            {
                int pixelId = startId + i * direction;
                if (pixelId > _pixelAmount)
                {
                    pixelId = pixelId - _pixelAmount;
                }

                img.SetPixel(pixelId, 0, Color.Black);
            } 
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
        _spiDevice.Dispose();
    }
}