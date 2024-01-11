using System.Drawing;
using GlideNGlow.Common.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.Protocol;

namespace GlideNGlow.Mqqt.Models;

public class LightStrip
{
    private int _pixelAmount;
    private readonly ILogger _logger;
    private readonly IOptionsMonitor<AppSettings> _appsettings;
    private readonly MqttHandler _mqttHandler;
    
    
    public LightStrip(ILogger<LightStrip> logger, IOptionsMonitor<AppSettings> appsettings,MqttHandler mqttHandler)
    {
        _logger = logger;
        _appsettings = appsettings;
        _pixelAmount = GetCurrentAppSettings().Strips.Aggregate(0, (i, strip) => i + strip.Leds);
        //_pixelAmount = size;
        
        if(_pixelAmount == 0)
        {
            _logger.LogError("Pixel amount is 0");
            //TODO: remove this line, appsettings isn't working atm.
            _pixelAmount = 300;
        }
        _mqttHandler = mqttHandler;
        
        
    }

    private AppSettings GetCurrentAppSettings()
    {
        return _appsettings.CurrentValue;
    }

    public void UpdateSettings()
    {
// #if DEBUG
//         _appSettings.Update(settings =>
//         {
//             settings.Strips = new List<LightStripData>()
//             {
//                 new LightStripData()
//                 {
//                     Leds = 300,
//                     Length = 500,
//                     DistanceTillNext = 0
//                 }
//                 
//             };
//         });
// #endif
        
        _pixelAmount = GetCurrentAppSettings().Strips.Aggregate(0, (i, Strips) => i + Strips.Leds);
        if(_pixelAmount == 0)
        {
            _logger.LogError("Pixel amount is 0");
            _pixelAmount = 300;
            //return;
        }

        SetPixelAmount(_pixelAmount).Wait();
        _logger.LogInformation($"Updated pixel amount to {_pixelAmount}");
    }
    
    public void SetPixel(int pixelId, byte r, byte g, byte b)
    {
        SetPixel(pixelId, Color.FromArgb(r, g, b));
    }
    
    public async Task SetPixel(int pixelId, Color color)
    {
        //todo: send pixel id and color over mqtt
        await _mqttHandler.SendMessage(TopicSetPixel, string.Format(PayloadSetPixel, color.R, color.G, color.B));
    }
    private const string TopicSetPixel = "esp32/strip/rgb";
    private const string PayloadSetPixel = "{{r: {0},  g:{1},  b:{2}}}";

    public async Task UpdateColors()
    {
        //todo: tell mqtt to update colors
        await _mqttHandler.SendMessage(TopicSetPixel, "");
    }
    private const string TopicUpdateColors = "esp32/strip/update";

    public async Task SetPixelAmount(int pixelAmount)
    {
        await _mqttHandler.SendMessage(TopicSetPixelAmount, string.Format(PayloadSetPixelAmount, pixelAmount));
    }
    private const string TopicSetPixelAmount = "esp32/strip/amount";
    private const string PayloadSetPixelAmount = "{0}";
    
    public void LightUpPixel(int pixelId,byte r, byte g, byte b)
    {
        SetPixel(pixelId, Color.FromArgb(r, g, b)).Wait();
        UpdateColors().Wait();
    }
    
    public void LightUpPixel(int pixelId,Color color)
    {
        SetPixel(pixelId, color).Wait();
        UpdateColors().Wait();
    }
    
    public void LightOutPixel(int pixelId)
    {
        SetPixel(pixelId, Color.Black).Wait();
        UpdateColors().Wait();
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
                SetPixel(pixelId, color).Wait();
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

                SetPixel(pixelId, color).Wait();
            } 
        }
        UpdateColors().Wait();
    }
    
    public void LightOutRange(int startId, int vector)
    {
        LightUpRange(startId, vector, Color.Black);
    }

    private Color ColorLerp(Color startColor, Color stopColor, int i, int length)
    {
        return Color.FromArgb(
            LerpIntDiff(startColor.R, stopColor.R, i, length),
            LerpIntDiff(startColor.G, stopColor.G, i, length),
            LerpIntDiff(startColor.B, stopColor.B, i, length)
        );
    }

    private int LerpIntDiff(int startInt, int stopInt, int i, int length)
    {
        int difference = stopInt - startInt;
        return (int)(startInt + ((difference * i )/ ((float)length)));
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
                SetPixel(pixelId, ColorLerp(startColor, stopColor, i, length)).Wait();
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

                SetPixel(pixelId, ColorLerp(startColor, stopColor, i, length)).Wait();
            } 
        }
        UpdateColors().Wait();
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
                SetPixel(pixelId, ColorLerp(startColor, stopColor, i, length)).Wait();
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

                SetPixel(pixelId, ColorLerp(startColor, stopColor, i, length)).Wait();
                Task.Delay(delayMilli);
            } 
        }
        UpdateColors().Wait();
        return Task.CompletedTask;
    }

    public void LightUpAll(Color color)
    {
        LightUpRange(0, _pixelAmount, color);
    }
    
    public void LightUpAll(int r, int g, int b)
    {
        LightUpRange(0, _pixelAmount, Color.FromArgb(r, g, b));
    }

    public void Pinkify()
    {
        LightUpAll(Color.HotPink);
        //255 50 60
    }

    public async Task Test2Async()
    {
        for(int i = 0; i<_pixelAmount; i++)
        {
            LightUpPixel(i, Color.Red);
            Task.Delay(100).Wait();
            if (i - 1 >= 0)
            {
                LightUpPixel(i-1, Color.Black);
            }
        }
        LightUpPixel(_pixelAmount-1, Color.Black);
        
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
}