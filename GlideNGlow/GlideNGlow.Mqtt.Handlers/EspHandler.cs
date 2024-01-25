using GlideNGlow.Common.Models;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqtt.Topics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options.Implementations;

namespace GlideNGlow.Mqqt.Handlers;

public class EspHandler
{
    private readonly MqttHandler _mqttHandler;
    private readonly ILogger _logger;
    private readonly IWritableOptions<AppSettings> _appSettings;
    private readonly Dictionary<string,LightButtons> _lightButtons = new();
    
    public event Func<int, Task>? ButtonPressedEvent;

    public EspHandler(ILogger<EspHandler> logger, IWritableOptions<AppSettings> appSettings, MqttHandler mqttHandler)
    {
        _mqttHandler = mqttHandler;
        _logger = logger;
        _appSettings = appSettings;
    }

    public async Task AddSubscriptions(CancellationToken cancellationToken)
    {
        await _mqttHandler.Subscribe(TopicEndpoints.SigninTopic, OnSignin, cancellationToken);
        await _mqttHandler.Subscribe(TopicEndpoints.TestTopic, OnTestSubscription, cancellationToken);
        await _mqttHandler.Subscribe(TopicEndpoints.ButtonTopic, OnButtonSubscription, cancellationToken);
    }
    
    public async Task RemoveSubscriptions(CancellationToken cancellationToken)
    {
        await _mqttHandler.Unsubscribe(TopicEndpoints.SigninTopic, cancellationToken);
        await _mqttHandler.Unsubscribe(TopicEndpoints.TestTopic, cancellationToken);
        await _mqttHandler.Unsubscribe(TopicEndpoints.ButtonTopic, cancellationToken);
    }

    public void HandleFileData(string macAddress)
    {
        //check if there is a Button in _appSettings.CurrentValue.Buttons with the same macAddress as the one we got
        LightButtonData? button = null;
        _appSettings.Update(s =>
        {
            button = s.Buttons.FirstOrDefault(x => x.MacAddress == macAddress);
            if (button == null)
            {
                //if there isn't, add a new one
                s.Buttons.Add(new LightButtonData()
                {
                    MacAddress = macAddress, ButtonNumber = -1, DistanceFromStart = 0
                });
                button = s.Buttons.Find(x => x.MacAddress == macAddress);
                if(button == null)
                    throw new Exception("button is still null");
            }
        });

        if (button is null)
            throw new NullReferenceException("Button is null after connect!");
        
        //now we are sure there is one, so we can add it to _lightButtons
        if (!_lightButtons.ContainsKey(macAddress))
        {
            _lightButtons.Add(button.MacAddress, new LightButtons(button, _logger)
            {
                MacAddress = macAddress
            });
        }
    }

#region Subscriptions

    private void OnSignin(string topic, string message)
    {
        var macAddress = topic.Split('/')[1];
        if (_lightButtons.ContainsKey(macAddress))
        {
            _logger.LogInformation($"Esp {macAddress} signed in: {message}");
        }
        else
        {
            HandleFileData(macAddress);
            // _lightButtons.Add(macAddress, new LightButtons(macAddress, _logger, SetRgb));
            _logger.LogInformation($"Esp {macAddress} resigned in: {message}");
            ReorderButtonIds();
        }
    }

    private void OnTestSubscription(string topic, string message)
    {
        var macAddress = topic.Split('/')[1];
        _logger.LogInformation($"Esp {macAddress} acknowledged: {message}");
        _lightButtons[macAddress].Responded = true;

        HandleFileData(macAddress);
        ReorderButtonIds();
    }

    private async Task OnButtonSubscription(string topic, string message)
    {
        var macAddress = topic.Split('/')[1];
        _logger.LogInformation("Esp {macAddress} button pressed: {message}", macAddress, message);
        if (_lightButtons.TryGetValue(macAddress, out var button))
            button.Pressed();
        else
            _logger.LogCritical("Button pressed but not registered uwu!"); // TODO this should notifiy clients
        
        if (ButtonPressedEvent is not null)
            await ButtonPressedEvent.Invoke(_lightButtons[macAddress].ButtonNumber ?? -1);
    }

#endregion
    
    public void AddButtonPressedEvent(Func<int, Task> callback)
    {
        ButtonPressedEvent += callback;
    }

    //function where you can register an action to get called whenever any button gets pressed, it'll also recieve the id of the button that got pressed
    public void AddPressedEvent(Action<int> callback)
    {
        foreach (var lightButton in _lightButtons)
        {
            lightButton.Value.AddPressedEvent(callback);
        }
    }
    
    //function where we go over each button and remove all callbacks
    public void RemovePressedEvent(Action<int> callback)
    {
        foreach (var lightButton in _lightButtons)
        {
            lightButton.Value.RemoveAllPressedEvents();
        }
    }
    
    //function to go over each button, compare their location(not done yet) and give them id's based on that order
    public void ReorderButtonIds()
    {
        var id = 0;
        //give each button an id based on their buttonLocation value, the lower their value the lower their id
        var keys = _lightButtons.Keys.ToList();
        _appSettings.Update(s =>
        {
            foreach (var key in keys.OrderBy(x => _lightButtons[x].DistanceFromStart))
            {
                _lightButtons[key].ButtonNumber = id;
                s.Buttons.Find(x => x.MacAddress == key)!.ButtonNumber = id;
                id++;
            }
        });
    }

    public async Task TestConnection(CancellationToken cancellationToken)
    {
        foreach (var esp in _lightButtons)
        {
            await _mqttHandler.SendMessage($"esp32/{esp.Key}/test", "test connection", cancellationToken);
        }
    }

    public async Task SetRgb(string macAddress,int r, int g, int b, CancellationToken cancellationToken)
    {
        await _mqttHandler.SendMessage($"esp32/{macAddress}/ledcircle", string.Format(TopicEndpoints.TopicRgb, r, g, b), cancellationToken);
    }
}