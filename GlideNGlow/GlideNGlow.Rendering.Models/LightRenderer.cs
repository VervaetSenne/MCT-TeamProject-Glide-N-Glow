using System.Drawing;
using System.Text;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GlideNGlow.Rendering.Models;

public class LightRenderer
{
    //private setter public getter for int _pixelAmount
    public int PixelAmount { get; private set; }
    private readonly ILogger _logger;
    private readonly IOptionsMonitor<AppSettings> _appsettings;
    private readonly MqttHandler _mqttHandler;
    //private setter public getter for List<Color> _lights
    public List<Color> Lights;
    
    public LightStripConverter LightStripConverter;

    public LightRenderer(ILogger<LightRenderer> logger, IOptionsMonitor<AppSettings> appsettings,
        MqttHandler mqttHandler)
    {
        _logger = logger;
        _appsettings = appsettings;
        _mqttHandler = mqttHandler;
        UpdateSettings();
        
        //create a colorlist with _pixelAmount amount of colors
        Lights = Enumerable.Repeat(Color.Black, PixelAmount).ToList();
    }

    private void UpdateSettings()
    {
        PixelAmount = GetCurrentAppSettings().Strips.Aggregate(0, (i, strip) => i + strip.Leds);
        //_pixelAmount = size;

        if (PixelAmount == 0)
        {
            _logger.LogError("Pixel amount is 0");
            PixelAmount = 300;
        }
        
        LightStripConverter = new LightStripConverter(GetCurrentAppSettings().Strips);
        
        SetStripSize(PixelAmount);
    }

    private AppSettings GetCurrentAppSettings()
    {
        return _appsettings.CurrentValue;
    }

    public void Render(RenderObject renderObject)
    {
        //check if renderObject implements ICustomRendering
        if (renderObject is ICustomRendering customRendering)
        {
            //if so, call the Render function
            customRendering.Render(this);
            return;
        }
        
        //check if renderObject implements IdRenderRenderObject
        if ((renderObject is IdRenderObject idRenderRenderObject))
        {
            RenderIdObject(idRenderRenderObject);
        }
    }

    private void RenderIdObject(IdRenderObject idRenderRenderObject)
    {
        int pos = idRenderRenderObject.GetOffset();
        //loop over renderObjects images and add them to the lightStrips list
        for (int i = 0; i < idRenderRenderObject.Image().Count; i++)
        {
            pos++;
            if (pos >= PixelAmount)
            {
                pos %= PixelAmount;
            }
            else if (pos < 0)
            {
                //if the key is negative, loop around
                pos = (pos % PixelAmount) + PixelAmount;
            }

            Lights[pos] = idRenderRenderObject.Image()[i];
        }
    }

    public void SetStripSize(int size)
    {
        PixelAmount = size;
        _mqttHandler.SendMessage(TopicSetStripSize, PixelAmount.ToString()).Wait();
    }

    private const string TopicSetStripSize = "esp32strip/config";
    private const string PayloadSetStripSize = "{0}";
    
    //on connect, send the strip size again
    public void AddOnConnectEvent()
    {
        _mqttHandler.Subscribe(TopicOnConnect, (topic, message) =>
        {
            _mqttHandler.SendMessage(TopicSetStripSize, PixelAmount.ToString()).Wait();
        }).Wait();
    }
    private const string TopicOnConnect = "esp32strip/connected";

    public void Clear()
    {
        //set entire _light
        Lights = Enumerable.Repeat(Color.Black, PixelAmount).ToList();
    }
    
    public async Task Show()
    {
        var payload = new StringBuilder();
        //add all colors as hexadecimals to the payload string
        foreach (Color color in Lights)
        {
            payload.Append(color.R.ToString("X2"));
            payload.Append(color.G.ToString("X2"));
            payload.Append(color.B.ToString("X2"));
        }
        
        _logger.LogInformation(payload.ToString());
        await _mqttHandler.SendMessage(TopicSetPixel, payload.ToString());
    }
    private const string TopicSetPixel = "esp32strip/led";
    // private const string PayloadSetPixel = "{0}";
    
    
    // not needed, esp will automatically update after sending it the colors
    // public async Task Update()
    // {
    //     await _mqttHandler.SendMessage(TopicSetPixel, "");
    // }
    // private const string TopicUpdateColors = "esp32/strip/update";
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