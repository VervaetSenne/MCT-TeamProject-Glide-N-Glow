using System.Drawing;
using System.Text;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GlideNGlow.Rendering.Models;

public class LightRenderer
{
    private int _pixelAmount;
    private readonly ILogger _logger;
    private readonly IOptionsMonitor<AppSettings> _appsettings;
    private readonly MqttHandler _mqttHandler;
    private List<Color> _lights;
    

    public LightRenderer(ILogger<LightRenderer> logger, IOptionsMonitor<AppSettings> appsettings,
        MqttHandler mqttHandler)
    {
        _logger = logger;
        _appsettings = appsettings;
        _mqttHandler = mqttHandler;
        _pixelAmount = GetCurrentAppSettings().Strips.Aggregate(0, (i, strip) => i + strip.Leds);
        //_pixelAmount = size;

        if (_pixelAmount == 0)
        {
            _logger.LogError("Pixel amount is 0");
            _pixelAmount = 300;
        }
        //create a colorlist with _pixelAmount amount of colors
        _lights = Enumerable.Repeat(Color.Black, _pixelAmount).ToList();
        SetStripSize(_pixelAmount);
    }

    public LightRenderer(List<Color> lights, MqttHandler mqttHandler)
    {
        _lights = lights;
        _mqttHandler = mqttHandler;
    }

    private AppSettings GetCurrentAppSettings()
    {
        return _appsettings.CurrentValue;
    }

    public void Render(RenderObject renderObject)
    {
        int pos = renderObject.GetOffset();
        //loop over renderObjects images and add them to the lightStrips list
        for (int i = 0; i < renderObject.Image().Count; i++)
        {
            pos++;
            //TODO: check if the key is in the range of the lightStrips list, if not loop around
            if (pos >= _pixelAmount)
            {
                pos %= _pixelAmount;
            }
            else if (pos < 0)
            {
                //if the key is negative, loop around
                pos = (pos % _pixelAmount) + _pixelAmount;
            }
            _lights[pos] = renderObject.Image()[i];
        }
    }
    
    public void SetStripSize(int size)
    {
        _pixelAmount = size;
        _mqttHandler.SendMessage(TopicSetStripSize, string.Format(PayloadSetStripSize, size)).Wait();
    }

    private const string TopicSetStripSize = "esp32strip/config";
    private const string PayloadSetStripSize = "{0}";

    public void Clear()
    {
        //set entire _light
        _lights = Enumerable.Repeat(Color.Black, _pixelAmount).ToList();
    }
    
    public async Task Show()
    {
        //todo: send pixel id and color over mqtt
        var payload = new StringBuilder();
        //add all colors as hexadecimals to the payload string
        foreach (Color color in _lights)
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
    
    public async Task Update()
    {
        //todo: tell mqtt to update colors
        await _mqttHandler.SendMessage(TopicSetPixel, "");
    }
    private const string TopicUpdateColors = "esp32/strip/update";
}