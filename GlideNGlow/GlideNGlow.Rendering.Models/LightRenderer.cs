using System.Drawing;
using System.Text;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Mqtt.Topics;
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

    private AppSettings AppSettings => _appsettings.CurrentValue;
    
    public List<Color> Lights { get; private set; }
    public LightStripConverter LightStripConverter { get; private set; } = null!;

    private LightRenderer(ILogger<LightRenderer> logger, IOptionsMonitor<AppSettings> appsettings,
        MqttHandler mqttHandler)
    {
        _logger = logger;
        _appsettings = appsettings;
        _mqttHandler = mqttHandler;
        
        //create a colorlist with _pixelAmount amount of colors
        Lights = Enumerable.Repeat(Color.Black, PixelAmount).ToList();
    }

    public static LightRenderer Create(ILogger<LightRenderer> logger, IOptionsMonitor<AppSettings> appsettings, MqttHandler mqttHandler, CancellationToken cancellationToken = default)
    {
        var renderer = new LightRenderer(logger, appsettings, mqttHandler);
        renderer.UpdateSettings(cancellationToken);
        return renderer;
    }

    private void UpdateSettings(CancellationToken cancellationToken)
    {
        PixelAmount = AppSettings.Strips.Aggregate(0, (i, strip) => i + strip.Leds);
        //_pixelAmount = size;

        if (PixelAmount == 0)
        {
            _logger.LogError("Pixel amount is 0");
            PixelAmount = 300;
        }
        
        LightStripConverter = new LightStripConverter(AppSettings.Strips);
        
        _ = SetStripSize(PixelAmount, cancellationToken);
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
        if (renderObject is IdRenderObject idRenderRenderObject)
        {
            RenderIdObject(idRenderRenderObject);
        }
    }

    private void RenderIdObject(IdRenderObject idRenderRenderObject)
    {
        var pos = idRenderRenderObject.GetOffset();
        //loop over renderObjects images and add them to the lightStrips list
        for (var i = 0; i < idRenderRenderObject.Image().Count; i++)
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

    public async Task SetStripSize(int size, CancellationToken cancellationToken)
    {
        PixelAmount = size;
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
        //set entire _light
        Lights = Enumerable.Repeat(Color.Black, PixelAmount).ToList();
    }
    
    public async Task Show(CancellationToken cancellationToken)
    {
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