using GlideNGlow.Common.Models;
using Microsoft.Extensions.Logging;

namespace GlideNGlow.Mqqt.Handlers;

public class LightButtons : LightButtonData
{
    private readonly ILogger _logger;
    
    public bool Responded { get; set; }
    
    public event Action<int>? PressedActions;

    public LightButtons(LightButtonData data, ILogger logger)
    {
        MacAddress = data.MacAddress;
        ButtonNumber = data.ButtonNumber;
        DistanceFromStart = data.DistanceFromStart;

        _logger = logger;
    }

    public void Pressed()
    {
        _logger.LogInformation("just pressed");
        
        //execute all PressedActions
        PressedActions?.Invoke(ButtonNumber ?? 0);
    }

    public void AddPressedEvent(Action<int>? callback)
    {
        PressedActions += callback;
    }
    
    public void RemovePressedEvent(Action<int>? callback)
    {
        PressedActions -= callback;
    }
    
    public void RemoveAllPressedEvents()
    {
        PressedActions = null;
    }
}