using System.Drawing;
using System.Text;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Common.Options.Extensions;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Mqtt.Topics;
using GlideNGlow.Rendering.Handlers.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options.Implementations;

namespace GlideNGlow.Rendering.Handlers;

public class LightRenderer
{
    //private setter public getter for int _pixelAmount
    public int PixelAmount { get; private set; }
    private readonly ILogger _logger;
    private readonly IWritableOptions<AppSettings> _appSettings;
    private readonly MqttHandler _mqttHandler;
    //private setter public getter for List<Color> _lights

    private bool _isDirty = true;

    private AppSettings AppSettings
    {
        get
        {
            AppSettings settings = null!;
            _appSettings.Update(s => settings = s, false);
            return settings;
        }
    }

    public List<Color> Lights { get; private set; }
    public LightStripConverter? LightStripConverter { get; private set; }

    private LightRenderer(ILogger<LightRenderer> logger, IWritableOptions<AppSettings> appSettings,
        MqttHandler mqttHandler)
    {
        _logger = logger;
        _appSettings = appSettings;
        _mqttHandler = mqttHandler;
        
        //create a colorlist with _pixelAmount amount of colors
        Lights = Enumerable.Repeat(Color.Black, PixelAmount).ToList();
        // if (PixelAmount <= 0)
        // {
        //     _logger.LogError("Pixel amount is 0 or less");
        // }
    }

    public static LightRenderer Create(ILogger<LightRenderer> logger, IWritableOptions<AppSettings> appsettings, MqttHandler mqttHandler, CancellationToken cancellationToken = default)
    {
        var renderer = new LightRenderer(logger, appsettings, mqttHandler);
        renderer.UpdateSettings(cancellationToken);
        return renderer;
    }
    
    public void OnFileChange(CancellationToken cancellationToken)
    {
        UpdateSettings(cancellationToken);
    }

    private void UpdateSettings(CancellationToken cancellationToken)
    {
        var oldPixelAmount = PixelAmount;
        PixelAmount = AppSettings.Strips.Aggregate(0, (i, strip) => i + strip.Leds);
        //_pixelAmount = size;

        if (PixelAmount <= 0)
        {
            _logger.LogError("Pixel amount is 0 or less");
            PixelAmount = 300;
        }
        
        if(LightStripConverter is null)
        {
            LightStripConverter = new LightStripConverter(AppSettings.Strips);
        }
        else
        {
            LightStripConverter.UpdateLightStripData(AppSettings.Strips);
        }
        
        if(oldPixelAmount != PixelAmount)
        {
            Lights.Clear();
            Lights = Enumerable.Repeat(Color.Black, PixelAmount).ToList();
        }
        SetStripSize(PixelAmount, cancellationToken).GetAwaiter().GetResult();
    }

    private async Task SetStripSize(int size, CancellationToken cancellationToken)
    {
        PixelAmount = size;
        await ResendStripSize(cancellationToken);
    }
    
    public async Task ResendStripSize(CancellationToken cancellationToken)
    {
        await _mqttHandler.SendMessage(TopicEndpoints.TopicSetStripSize, PixelAmount.ToString(), cancellationToken);
    }
    
    //on connect, send the strip size again
    public async Task AddOnConnectEvent(CancellationToken cancellationToken)
    {
        await _mqttHandler.Subscribe(TopicEndpoints.TopicOnConnect, async (_, _) =>
        {
            await _mqttHandler.SendMessage(TopicEndpoints.TopicSetStripSize, PixelAmount.ToString(), cancellationToken);
        }, cancellationToken);
    }

    public void Clear()
    {
        //_isDirty = true;
        //set entire _light
        Lights = Enumerable.Repeat(Color.Black, PixelAmount).ToList();
    }
    
    public async Task ShowAsync(CancellationToken cancellationToken)
    {
        if (!_isDirty) return;
        _isDirty = false;
        var payload = new StringBuilder();
        //add all colors as hexadecimals to the payload string
        foreach (var color in Lights)
        {
            payload.Append(color.R.ToString("X2"));
            payload.Append(color.G.ToString("X2"));
            payload.Append(color.B.ToString("X2"));
        }
        
#if DEBUG
        _logger.LogInformation(payload.ToString());
#endif
        
        await _mqttHandler.SendMessage(TopicEndpoints.TopicSetPixel, payload.ToString(), cancellationToken);
    }
    
    // not needed, esp will automatically update after sending it the colors
    // public async Task Update()
    // {
    //     await _mqttHandler.SendMessage(TopicSetPixel, "");
    // }
    // private const string TopicUpdateColors = "esp32/strip/update";
    public void MakeDirty()
    {
        _isDirty = true;
    }
    public void SetPixel(int drawPosition, Color color)
    {
        if (drawPosition >= PixelAmount)
        {
            drawPosition %= PixelAmount;
        }
        else if (drawPosition < 0)
        {
            //if the key is negative, loop around
            drawPosition = (drawPosition % PixelAmount) + PixelAmount;
        }
        Lights[drawPosition] = color;
    }
}