using System.Drawing;
using GlideNGlow.Common.Models;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Common.Options.Extensions;
using GlideNGlow.Mqtt.Topics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options.Implementations;

namespace GlideNGlow.Mqqt.Handlers;

public class LightButtonHandler
{
    private readonly MqttHandler _mqttHandler;
    private readonly ILogger _logger;
    private readonly IWritableOptions<AppSettings> _appSettings;

    private AppSettings AppSettings => _appSettings.GetCurrentValue();
    
    public readonly Dictionary<string,LightButtons> LightButtons = new();

    public event Func<int, Task>? ButtonPressedEvent;

    public LightButtonHandler(ILogger<LightButtonHandler> logger, IWritableOptions<AppSettings> appSettings, MqttHandler mqttHandler)
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

    //TODO: Call this function after project startup
    public void OnStartupFileChange()
    {
        //on startup we want to go over each button and put all buttonNumbers that are -1 on -3 and all others on -2
        //Put all buttonNumbers that are -1 on -3 and all others on -2
        _appSettings.Update(settings =>
        {
            foreach (var button in settings.Buttons)
            {
                if (button.ButtonNumber == -1)
                {
                    button.ButtonNumber = -3;
                }
                else
                {
                    button.ButtonNumber = -2;
                }
            }
        });
    }
    
    private void SigninNewButton(string macAddress)
    {
        //check if there is a Button in _appSettings.CurrentValue.Buttons with the same macAddress as the one we got
        LightButtonData? button = null;
        _appSettings.Update(settings =>
        {
            //check if there is a Button in _appSettings.CurrentValue.Buttons with the same macAddress as the one we got
            button = settings.Buttons.FirstOrDefault(x => x.MacAddress == macAddress);
            if (button is null)
            {
                //there isn't, add a new one and put it's buttonNumber on -1 and notify the client to give it a location and return
                settings.Buttons.Add(new LightButtonData()
                {
                    MacAddress = macAddress, ButtonNumber = -1, DistanceFromStart = 0
                });
                //TODO: notify the client to give it a location
                return;
            }
            
            //check if the buttonNumber is -2,
            if (button.ButtonNumber == -2)
            {
                //add it to LightButtons (we'll change the number later)
                LightButtons.Add(button.MacAddress, new LightButtons(button, _logger)
                {
                    MacAddress = macAddress
                });
                
                //look at all buttons 0 or above and give them a number based on their distance from the start
                //make a list of all buttons that have a buttonNumber of 0 or above
                var buttons = settings.Buttons.Where(x => x.ButtonNumber >= 0).ToList();
                
                //sort the list based on their distanceFromStart value
                buttons.Sort((x, y) => Convert.ToInt32(x.DistanceFromStart.GetValueOrDefault() - y.DistanceFromStart.GetValueOrDefault()));
                
                //go over each button and give them a buttonNumber based on their location in the list
                for (var i = 0; i < buttons.Count; i++)
                {
                    buttons[i].ButtonNumber = i;
                    //also change the buttonNumber of the button in LightButtons
                    LightButtons[buttons[i].MacAddress].ButtonNumber = i;
                }

                return;
            }

                    
            //if buttonNumber is 0 or above, the button just reconnected, so we don't need to do anything besides notifying the client it reconnected
            if (button.ButtonNumber >= 0)
            {
                //TODO: notify the client a button reconnected
                return;
            }
            
            
            //if buttonNumber is -3, the button doesn't have settings yet, change its number to -1 and notifying the client it reconnected

            if (button.ButtonNumber == -3)
            {
                button.ButtonNumber = -1;
                return;
            }
        });
        
    }

    //TODO: Call this function when the file changes
    public void OnFileChange()
    {
        LightButtonData? button = null;
        _appSettings.Update(settings =>
        {
            bool dirtyFlag = false;
            //make a list of all buttons in _appSettings.CurrentValue.Buttons, ignore all -2's and -3's
            var buttons = settings.Buttons.Where(x => x.ButtonNumber >= -1).ToList();
            //for each button in the list, if its buttonNumber is -1 check if its in LightButtons, if it is, remove it from LightButtons and put a dirtyFlag on true
            foreach (var buttonData in buttons.Where(buttonData => buttonData.ButtonNumber == -1))
            {
                if (LightButtons.ContainsKey(buttonData.MacAddress))
                {
                    LightButtons.Remove(buttonData.MacAddress);
                    dirtyFlag = true;
                }
            }
            //for each button in the list, if its buttonNumber 0 or higher check if its in LightButtons, if it isn't, add it to LightButtons and put a dirtyFlag on true
            foreach (var buttonData in buttons.Where(buttonData => buttonData.ButtonNumber >= 0))
            {
                if (!LightButtons.ContainsKey(buttonData.MacAddress))
                {
                    LightButtons.Add(buttonData.MacAddress, new LightButtons(buttonData, _logger)
                    {
                        MacAddress = buttonData.MacAddress
                    });
                    dirtyFlag = true;
                }
                //check if the distance is the same else, update it and put a dirtyFlag on true
                if (LightButtons[buttonData.MacAddress].DistanceFromStart != buttonData.DistanceFromStart)
                {
                    LightButtons[buttonData.MacAddress].DistanceFromStart = buttonData.DistanceFromStart;
                    dirtyFlag = true;
                }
            }
            
            //if dirtyFlag is true, go over each button in LightButtons and give them a buttonNumber based on their location in the list
            if (dirtyFlag)
            {
                var activeButtons = buttons.Where(x => x.ButtonNumber >= 0).ToList();
                var gotReshuffled = false;
                //sort the list based on their distanceFromStart value
                activeButtons.Sort((x, y) => Convert.ToInt32(x.DistanceFromStart.GetValueOrDefault() - y.DistanceFromStart.GetValueOrDefault()));
                
                //go over each button and give them a buttonNumber based on their location in the list
                for (var i = 0; i < activeButtons.Count; i++)
                {
                    //if the buttonNumber is already the same as i, skip it
                    if (activeButtons[i].ButtonNumber != i)
                    {
                        activeButtons[i].ButtonNumber = i;
                        //also change the buttonNumber of the button in LightButtons
                        LightButtons[activeButtons[i].MacAddress].ButtonNumber = i; 
                        gotReshuffled = true;
                    }
                }
                if(gotReshuffled==true)
                {
                    //TODO: notify the client that the button ids got changed
                }
            }
            
        });
    }
    

#region Subscriptions

    private void OnSignin(string topic, string message)
    {
        var macAddress = topic.Split('/')[1];
        if (LightButtons.ContainsKey(macAddress))
        {
            _logger.LogInformation($"Esp {macAddress} signed in: {message}");
        }
        else
        {
            SigninNewButton(macAddress);
            // _lightButtons.Add(macAddress, new LightButtons(macAddress, _logger, SetRgb));
            _logger.LogInformation($"Esp {macAddress} resigned in: {message}");
        }
    }

    private void OnTestSubscription(string topic, string message)
    {
        var macAddress = topic.Split('/')[1];
        _logger.LogInformation($"Esp {macAddress} acknowledged: {message}");
        LightButtons[macAddress].Responded = true;

        SigninNewButton(macAddress);
    }

    private async Task OnButtonSubscription(string topic, string message)
    {
        var macAddress = topic.Split('/')[1];
        _logger.LogInformation("Esp {macAddress} button pressed: {message}", macAddress, message);
        if (LightButtons.TryGetValue(macAddress, out var button))
            button.Pressed();
        else
            _logger.LogCritical("Button pressed but not registered uwu!"); // TODO this should notifiy clients
        
        if (ButtonPressedEvent is not null)
            await ButtonPressedEvent.Invoke(LightButtons[macAddress].ButtonNumber ?? -1);
    }

#endregion
    
    public void AddButtonPressedEvent(Func<int, Task> callback)
    {
        ButtonPressedEvent += callback;
    }

    //function where you can register an action to get called whenever any button gets pressed, it'll also recieve the id of the button that got pressed
    public void AddPressedEvent(Action<int> callback)
    {
        foreach (var lightButton in LightButtons)
        {
            lightButton.Value.AddPressedEvent(callback);
        }
    }
    
    //function where we go over each button and remove all callbacks
    public void RemovePressedEvent(Action<int> callback)
    {
        foreach (var lightButton in LightButtons)
        {
            lightButton.Value.RemoveAllPressedEvents();
        }
    }

    public async Task TestConnection(CancellationToken cancellationToken)
    {
        foreach (var esp in LightButtons)
        {
            await _mqttHandler.SendMessage($"esp32/{esp.Key}/test", "test connection", cancellationToken);
        }
    }

    public async Task SetRgb(string macAddress,int r, int g, int b, CancellationToken cancellationToken)
    {
        await _mqttHandler.SendMessage($"esp32/{macAddress}/ledcircle", string.Format(TopicEndpoints.TopicRgb, r, g, b), cancellationToken);
    }
    
    public async Task SetRgb(string macAddress, Color color, CancellationToken cancellationToken)
    {
        await SetRgb(macAddress, color.R, color.G, color.B, cancellationToken);
    }
    
    public async Task SetRgb(int buttonId, int r, int g, int b, CancellationToken cancellationToken)
    {
        string macAddress = GetMacAddress(buttonId, cancellationToken).Result;
        await SetRgb(macAddress, r, g, b, cancellationToken);
    }
    
    public async Task SetRgb(int buttonId, Color color, CancellationToken cancellationToken)
    {
        await SetRgb(buttonId, color.R, color.G, color.B, cancellationToken);
    }
    
    public async Task SetAllRgb(int r, int g, int b, CancellationToken cancellationToken)
    {
        foreach (var esp in LightButtons)
        {
            await SetRgb(esp.Key, r, g, b, cancellationToken);
        }
    }
    
    public async Task SetAllRgb(Color color, CancellationToken cancellationToken)
    {
        foreach (var esp in LightButtons)
        {
            await SetRgb(esp.Key, color.R, color.G, color.B, cancellationToken);
        }
    }
    
    public async Task<string> GetMacAddress(int buttonId, CancellationToken cancellationToken)
    {
        //check all in LightButtons to see if any match the buttonId
        foreach (var lightButton in LightButtons)
        {
            if (lightButton.Value.ButtonNumber == buttonId)
            {
                return lightButton.Key;
            }
        }

        return "";
    }
    
}