using System.Device.Spi;
using System.Drawing;
using GlideNGlow.Common.Models;
using Iot.Device.Ws28xx;
using GlideNGlow.Common.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options.Implementations;

namespace GlideNGlow.GPIO.Models;

public class SpiDeviceHandler : IDisposable
{
    private readonly SpiDevice _spiDevice;
    private Ws2812b _ws2812B;
    private readonly IWritableOptions<AppSettings> _appSettings;
    private int _pixelAmount;
    private readonly ILogger _logger;
    
    public SpiDeviceHandler(IWritableOptions<AppSettings> appSettings,ILogger<SpiDeviceHandler> logger)
    {
        _logger = logger;
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
#if DEBUG
        _appSettings.Update(settings =>
        {
            settings.Strips = new List<LightStripData>()
            {
                new LightStripData()
                {
                    Leds = 300,
                    Length = 500,
                    DistanceTillNext = 0
                }
                
            };
        });
#endif
        
        _pixelAmount = GetCurrentAppSettings().Strips.Aggregate(0, (i, Strips) => i + Strips.Leds);
        _pixelAmount = 300;
        if(_pixelAmount == 0)
        {
            _logger.LogError("Pixel amount is 0");
            return;
        }
        
        _ws2812B = new Ws2812b(_spiDevice, _pixelAmount);
        _logger.LogInformation($"Updated pixel amount to {_pixelAmount}");
    }
    
    private AppSettings GetCurrentAppSettings()
    {
        return _appSettings.CurrentValue;
    }
    
    public void SetPixel(int pixelId, byte r, byte g, byte b)
    {
        SetPixel(pixelId, Color.FromArgb(r, g, b));
    }
    
    public void SetPixel(int pixelId, Color color)
    {
        _ws2812B.Image.SetPixel(pixelId, 0, color);
        
    }

    public void UpdateColors()
    {
        _ws2812B.Update();
    }
    
    public void LightUpPixel(int pixelId,byte r, byte g, byte b)
    {
        SetPixel(pixelId, Color.FromArgb(r, g, b));
        UpdateColors();
    }
    
    public void LightUpPixel(int pixelId,Color color)
    {
        SetPixel(pixelId, color);
        UpdateColors();
    }
    
    public void LightOutPixel(int pixelId)
    {
        SetPixel(pixelId, Color.Black);
        UpdateColors();
    }

    public void LightUpRange(int startId, int vector, byte r, byte g, byte b)
    {
        LightUpRange(startId, vector, Color.FromArgb(r, g, b));
    }
    
    public void LightUpRange(int startId, int vector, Color color)
    {
        var direction = (vector - startId) / Math.Abs(vector - startId);
        var length = Math.Abs(vector - startId);

        if (direction < 0)
        {
            for (int i = 0; i < length; i++)
            {
                int pixelId = startId + i * direction;
                if (pixelId < 0)
                {
                    pixelId += _pixelAmount;
                }
                SetPixel(pixelId, color);
            } 
        }
        else
        {
            for (int i = 0; i < length; i++)
            {
                int pixelId = startId + i * direction;
                if (pixelId > _pixelAmount)
                {
                    pixelId -= _pixelAmount;
                }

                SetPixel(pixelId, color);
            } 
        }
        UpdateColors();
    }
    
    public void LightOutRange(int startId, int vector)
    {
        LightUpRange(startId, vector, Color.Black);
    }

    private Color ColorLerp(Color startColor, Color stopColor, int i, int length)
    {
        return Color.FromArgb(
            (int)Math.Round(startColor.R + (stopColor.R - startColor.R) * ((double)i / length)),
            (int)Math.Round(startColor.G + (stopColor.G - startColor.G) * ((double)i / length)),
            (int)Math.Round(startColor.B + (stopColor.B - startColor.B) * ((double)i / length))
        );
    }
    
    public void LightUpLerp(int startId, int vector, Color startColor, Color stopColor)
    {
        var direction = (vector - startId) / Math.Abs(vector - startId);
        var length = Math.Abs(vector - startId);
        Color color;

        if (direction < 0)
        {
            for (int i = 0; i < length; i++)
            {
                int pixelId = startId + i * direction;
                if (pixelId < 0)
                {
                    pixelId += _pixelAmount ;
                }
                SetPixel(pixelId, ColorLerp(startColor, stopColor, i, length));
            } 
        }
        else
        {
            for (int i = 0; i < length; i++)
            {
                int pixelId = startId + i * direction;
                if (pixelId > _pixelAmount)
                {
                    pixelId -= _pixelAmount;
                }

                SetPixel(pixelId, ColorLerp(startColor, stopColor, i, length));
            } 
        }
        UpdateColors();
    }
    
    public Task LightUpGrowAsync(int startId, int vector, Color startColor, Color stopColor, int delayMilli)
    {
        var direction = (vector - startId) / Math.Abs(vector - startId);
        var length = Math.Abs(vector - startId);
        Color color;

        if (direction < 0)
        {
            for (int i = 0; i < length; i++)
            {
                int pixelId = startId + i * direction;
                if (pixelId < 0)
                {
                    pixelId += _pixelAmount;
                }
                SetPixel(pixelId, ColorLerp(startColor, stopColor, i, length));
                Task.Delay(delayMilli);
            } 
        }
        else
        {
            for (int i = 0; i < length; i++)
            {
                int pixelId = startId + i * direction;
                if (pixelId > _pixelAmount)
                {
                    pixelId -= _pixelAmount;
                }

                SetPixel(pixelId, ColorLerp(startColor, stopColor, i, length));
                Task.Delay(delayMilli);
            } 
        }
        UpdateColors();
        return Task.CompletedTask;
    }

    public void LightUpAll(Color color)
    {
        LightUpRange(0, _pixelAmount, color);
    }

    public void Pinkify()
    {
        LightUpAll(Color.HotPink);
        //255 50 60
    }

    public Task TestAsync()
    {
        if(_pixelAmount == 0)
        {
            UpdateSettings();
            if (_pixelAmount == 0)
            {
                _logger.LogError("Pixel amount is 0");
                return Task.CompletedTask;
            }
        }
        Random randomGen = new Random();
        for(int i = 0; i < 50; i++)
        {
            int maxColorBrightness = 150;
            //generate random color
            Color color = Color.FromArgb(randomGen.Next(0, maxColorBrightness), randomGen.Next(0, maxColorBrightness), randomGen.Next(0, maxColorBrightness));
            Color color2 = Color.FromArgb(randomGen.Next(0, maxColorBrightness), randomGen.Next(0, maxColorBrightness), randomGen.Next(0, maxColorBrightness));
            //generate random int from -50 till 50 that isn't 0
            int randomInt = randomGen.Next(-50, 50);
            while(randomInt == 0)
            {
                randomInt = randomGen.Next(-50, 50);
            }
            int start = randomGen.Next(0,_pixelAmount);
            LightUpGrowAsync(start, randomInt, color, color2, 100).Wait();
            Task.Delay(10000);
        }
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _spiDevice.Dispose();
    }
}