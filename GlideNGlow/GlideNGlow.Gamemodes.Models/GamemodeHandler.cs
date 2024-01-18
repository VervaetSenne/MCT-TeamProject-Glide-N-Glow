using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;
using Microsoft.Extensions.Options;

namespace GlideNGlow.Gamemodes.Models;

public class GamemodeHandler
{
    Gamemode _gamemode;
    private readonly LightRenderer _lightRenderer;
    private readonly EspHandler _espHandler;

    public GamemodeHandler(LightRenderer lightRenderer, IOptionsMonitor<AppSettings> appSettings, EspHandler espHandler)
    {
        _lightRenderer = lightRenderer;
        _espHandler = espHandler;
        //calculate length of strip from appSettings
        var currentValueStrips = appSettings.CurrentValue.Strips;
        float length = 0;
        foreach (var strip in currentValueStrips)
        {
            length += strip.Length + strip.DistanceFromLast;
        }
        _gamemode = new GhostRace(espHandler, length  ,15);
    }
    
    public async Task Update(float deltaSeconds)
    {
        await _gamemode.Update(deltaSeconds);
    }
    
    public async Task Render()
    {
        foreach (var renderObject in _gamemode.GetRenderObjects())
        {
            _lightRenderer.Render(renderObject);
        }

        await _lightRenderer.Show();
    }
    
    //TODO: assemble gamemodes from frontend
    public void SetGamemode(Gamemode gamemode)
    {
        
        _gamemode = gamemode;
    }
    
    public async Task AddSubscriptions()
    {
        await _espHandler.AddButtonPressedEvent(Input);
    }
    
    public void Input(int id)
    {
        _gamemode.Input(id);
    }
    
    
}