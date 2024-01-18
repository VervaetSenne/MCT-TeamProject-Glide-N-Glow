using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;
using Microsoft.Extensions.Options;

namespace GlideNGlow.Gamemodes.Models;

public class GamemodeHandler
{
    private Gamemode _gamemode;
    private readonly LightRenderer _lightRenderer;
    private readonly EspHandler _espHandler;

    public GamemodeHandler(LightRenderer lightRenderer, IOptionsMonitor<AppSettings> appSettings, EspHandler espHandler)
    {
        _lightRenderer = lightRenderer;
        _espHandler = espHandler;

        var currentAppSettings = appSettings.CurrentValue;
        var currentValueStrips = currentAppSettings.Strips;
        var length = currentValueStrips.Sum(strip => strip.Length + strip.DistanceFromLast);
        _gamemode = new GhostRace(currentAppSettings, espHandler, length  ,15);
    }
    
    public void Start()
    {
        _gamemode.Start();
        AddSubscriptions();
    }
    
    public async Task UpdateAsync(float deltaSeconds)
    {
        await _gamemode.Update(deltaSeconds);
    }
    
    public async Task RenderAsync(CancellationToken cancellationToken)
    {
        if(_gamemode.GetRenderObjects().Count == 0)
            return;
        _lightRenderer.Clear();
        foreach (var renderObject in _gamemode.GetRenderObjects())
        {
            _lightRenderer.Render(renderObject);
        }
        
        await _lightRenderer.ShowAsync(cancellationToken);
    }
    
    //TODO: assemble gamemodes from frontend
    public void SetGamemode(Gamemode gamemode)
    {
        _gamemode = gamemode;
    }
    
    public void AddSubscriptions()
    {
        _espHandler.AddButtonPressedEvent(Input);
    }
    
    public void Input(int id)
    {
        _gamemode.Input(id);
    }
}