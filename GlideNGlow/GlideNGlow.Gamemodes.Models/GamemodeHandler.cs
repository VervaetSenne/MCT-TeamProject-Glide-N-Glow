using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Gamemodes.Models.Abstractions;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;
using Microsoft.Extensions.Options;

namespace GlideNGlow.Gamemodes.Models;

public class GamemodeHandler
{
    private IGamemode _gamemode;
    private readonly LightRenderer _lightRenderer;
    private readonly EspHandler _espHandler;

    public GamemodeHandler(LightRenderer lightRenderer, IOptionsMonitor<AppSettings> appSettings, EspHandler espHandler)
    {
        _lightRenderer = lightRenderer;
        _espHandler = espHandler;

        var currentAppSettings = appSettings.CurrentValue;
        var currentValueStrips = currentAppSettings.Strips;
        var length = currentValueStrips.Sum(strip => strip.Length + strip.DistanceFromLast);
        _gamemode = new GhostRace(currentAppSettings, _espHandler, length  ,15);
    }
    
    public void Start()
    {
        _gamemode.Initialize();
        AddSubscriptions();
    }
    
    public async Task UpdateAsync(TimeSpan timeSpan)
    {
        await _gamemode.UpdateAsync(timeSpan);
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
    public void SetGamemode(IGamemode gamemode)
    {
        _gamemode = gamemode;
    }
    
    public void AddSubscriptions()
    {
        _espHandler.AddButtonPressedEvent(Input);
    }
    
    public void Input(int id)
    {
        _gamemode.ButtonPressed(id);
    }
}