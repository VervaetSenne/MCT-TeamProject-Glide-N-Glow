using Microsoft.Extensions.Logging;
using MQTTnet.Protocol;

namespace GlideNGlow.Mqqt.Models;

public class EspHandler : IAsyncDisposable
{

    private readonly MqttHandler _mqttHandler;
    private readonly ILogger _logger;
    private Dictionary<string,LightButtons> _lightButtons = new Dictionary<string,LightButtons>();
    
    private const string TopicRgb = "{{r: {0},  g:{1},  b:{2}}}";

    public EspHandler(ILogger<EspHandler> logger, MqttHandler mqttHandler)
    {
        _mqttHandler = mqttHandler;
        _logger = logger;
    }

    #region Subscriptions

    public async Task AddSubscriptions()
    {
        await AddSignin();
        await AddTestConnectionsSubscription();
        await AddButtonSubscription();
        //await _mqttHandler.Subscribe("eee", (topic, message) => { _logger.LogInformation("eee");
        //}, MqttQualityOfServiceLevel.AtLeastOnce);
    }


    public async Task AddSignin()
    {
        string signinTopic = "esp32/+/connected";
        await _mqttHandler.Subscribe(signinTopic, (topic, message) =>
        {
            string macAddress = topic.Split('/')[1];
            if(_lightButtons.ContainsKey(macAddress))
            {
                _logger.LogInformation($"Esp {macAddress} signed in: {message}");
            }
            else
            {
                _lightButtons.Add(macAddress, new LightButtons(macAddress, _logger, SetRgb));
                _logger.LogInformation($"Esp {macAddress} resigned in: {message}");
            }

        }, MqttQualityOfServiceLevel.AtLeastOnce);

    }

    public async Task AddTestConnectionsSubscription()
    {
        string testTopic = "esp32/+/acknowledge";
        await _mqttHandler.Subscribe(testTopic, (topic, message) =>
        {
            string macAddress = topic.Split('/')[1];
            //_logger.LogInformation(topic);
            _logger.LogInformation($"Esp {macAddress} acknowledged: {message}");
            _lightButtons[macAddress].Responded = true;
        }, MqttQualityOfServiceLevel.AtLeastOnce);
    }

    public async Task AddButtonSubscription()
    {
        string buttonTopic = "esp32/+/button";
        await _mqttHandler.Subscribe(buttonTopic, (topic, message) =>
        {
            string macAddress = topic.Split('/')[1];
            //_logger.LogInformation(topic);
            _logger.LogInformation($"Esp {macAddress} button pressed: {message}");
            _lightButtons[macAddress]?.Pressed();
        }, MqttQualityOfServiceLevel.AtLeastOnce);
    }

    #endregion

    public async Task TestConnection()
    {
        foreach (var esp in _lightButtons)
        {
            await _mqttHandler.SendMessage($"esp32/{esp.Key}/test", "test connection");
        }
    }

    public async Task SetRgb(string macAddress,int r, int g, int b)
    {

        await _mqttHandler.SendMessage($"esp32/{macAddress}/ledcircle", string.Format(TopicRgb, r, g, b));
    }




    public async ValueTask DisposeAsync()
    {
        await _mqttHandler.DisposeAsync();
    }
}