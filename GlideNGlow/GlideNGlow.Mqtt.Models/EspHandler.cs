using GlideNGlow.Common.Models;
using GlideNGlow.Common.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.Protocol;
using System.Linq;

namespace GlideNGlow.Mqqt.Models;

public class EspHandler : IDisposable
{

    private readonly MqttHandler _mqttHandler;
    private readonly ILogger _logger;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private Dictionary<String,LightButtons> _lightButtons = new Dictionary<string,LightButtons>();
    
    public event Action<int>? ButtonPressedEvent;

    public EspHandler(ILogger<EspHandler> logger, IOptionsMonitor<AppSettings> appSettings,MqttHandler mqttHandler)
    {
        
        _mqttHandler = mqttHandler;
        _logger = logger;
        _appSettings = appSettings;
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
                HandleFileData(macAddress).Wait();
                // _lightButtons.Add(macAddress, new LightButtons(macAddress, _logger, SetRgb));
                _logger.LogInformation($"Esp {macAddress} resigned in: {message}");
                ReorderButtonIds().Wait();
            }
            
        }, MqttQualityOfServiceLevel.AtLeastOnce);

    }
    
    
    
    public AppSettings GetAppSettings()
    {
        return _appSettings.CurrentValue;
    }

    public async Task HandleFileData(String macAddress)
    {
        //check if there is a Button in _appSettings.CurrentValue.Buttons with the same macAddress as the one we got
        LightButtonData? button = GetAppSettings().Buttons.Find(x => x.MacAddress == macAddress);
        if (button == null)
        {
            //if there isn't, add a new one
            _appSettings.CurrentValue.Buttons.Add(new LightButtonData()
            {
                MacAddress = macAddress, ButtonNumber = -1, DistanceFromStart = 0
            });
            button = GetAppSettings().Buttons.Find(x => x.MacAddress == macAddress);
            if(button == null)
                throw new Exception("button is still null");
        }
        
        //now we are sure there is one, so we can add it to _lightButtons

        if (_lightButtons.ContainsKey(macAddress))
        {
            _lightButtons.Add(button!.MacAddress, new LightButtons(button,_logger, SetRgb));
        }
    }

    public async Task AddTestConnectionsSubscription()
    {
        const string testTopic = "esp32/+/acknowledge";
        await _mqttHandler.Subscribe(testTopic, (topic, message) =>
        {
            string macAddress = topic.Split('/')[1];
            //_logger.LogInformation(topic);
            _logger.LogInformation($"Esp {macAddress} acknowledged: {message}");
            _lightButtons[macAddress].Responded = true;

            HandleFileData(macAddress).Wait();
            ReorderButtonIds().Wait();
        }, MqttQualityOfServiceLevel.AtLeastOnce);
    }

    public async Task AddButtonSubscription()
    {
        await _mqttHandler.Subscribe(ButtonTopic, (topic, message) =>
        {
            string macAddress = topic.Split('/')[1];
            //_logger.LogInformation(topic);
            _logger.LogInformation($"Esp {macAddress} button pressed: {message}");
            _lightButtons[macAddress]?.Pressed();
            ButtonPressedEvent?.Invoke(_lightButtons[macAddress].ButtonNumber ?? -1);
        }, MqttQualityOfServiceLevel.AtLeastOnce);
    }
    private const string ButtonTopic = "esp32/+/button";

    #endregion
    
    public async Task AddButtonPressedEvent(Action<int> callback)
    {
        ButtonPressedEvent += callback;
    }

    //function where you can register an action to get called whenever any button gets pressed, it'll also recieve the id of the button that got pressed
    public async Task AddPressedEvent(Action<int> callback)
    {
        foreach (var lightButton in _lightButtons)
        {
            await lightButton.Value.AddPressedEvent(callback);
        }
    }
    
    //function where we go over each button and remove all callbacks
    public async Task RemovePressedEvent(Action<int> callback)
    {
        foreach (var lightButton in _lightButtons)
        {
            await lightButton.Value.RemoveAllPressedEvents();
        }
    }
    
    //function to go over each button, compare their location(not done yet) and give them id's based on that order
    public async Task ReorderButtonIds()
    {
        int id = 0;
        //give each button an id based on their buttonLocation value, the lower their value the lower their id
        List<String> keys = _lightButtons.Keys.ToList();
        foreach (var key in keys.OrderBy(x => _lightButtons[x].DistanceFromStart))
        {
            _lightButtons[key].ButtonNumber = id;
            GetAppSettings().Buttons.Find(x => x.MacAddress == key)!.ButtonNumber = id;
            id++;
        }
    }

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
    private const string TopicRgb = "{{r: {0},  g:{1},  b:{2}}}";
    
    public void Dispose()
    {
        _mqttHandler.Dispose();
    }
}