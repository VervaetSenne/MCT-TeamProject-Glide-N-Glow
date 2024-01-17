using GlideNGlow.Common.Models;
using Microsoft.Extensions.Logging;

namespace GlideNGlow.Mqqt.Models;

public class LightButtons : LightButtonData
{

    private readonly ILogger _logger;
    private readonly Func<string, int, int, int,Task> _rgbAction;
    public bool Responded { get; set; }
    public event Action<int>? PressedActions;
    

    public LightButtons(LightButtonData data, ILogger logger, Func<string, int, int, int,Task> rgbAction)
    {
        MacAddress = data.MacAddress;
        ButtonNumber = data.ButtonNumber;
        ButtonLocation = data.ButtonLocation;
        
        _logger = logger;
        _rgbAction = rgbAction;
    }

    public async Task Pressed()
    {
        _logger.LogInformation("just pressed");
        Random random = new Random();
        //TODO: remove random colors
        await _rgbAction(MacAddress, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
        
        //await _rgbAction(MacAddress, 0, 255, 0);
        
        //execute all PressedActions
        PressedActions?.Invoke(ButtonNumber);
    }

    public async Task AddPressedEvent(Action<int>? callback)
    {
        PressedActions += callback;
    }
    
    public async Task RemovePressedEvent(Action<int>? callback)
    {
        PressedActions -= callback;
    }
    
    public async Task RemoveAllPressedEvents()
    {
        PressedActions = null;
    }
    
    
    
    
    
    
}