using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Gamemodes.Models.Abstractions;
using GlideNGlow.Gamemodes.Modes;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Handlers;
using Microsoft.Extensions.Options;

namespace GlideNGlow.Gamemodes.Handlers;

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
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _gamemode.Initialize();
        await AddSubscriptionsAsync(cancellationToken);
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
            renderObject.Render(_lightRenderer);
        }
        
        await _lightRenderer.ShowAsync(cancellationToken);
    }
    
    //TODO: assemble gamemodes from frontend
    public void SetGamemode(IGamemode gamemode)
    {
        _gamemode = gamemode;
    }

    private void StopGame()
    {
        _gamemode.Stop();
    }
    
    public async Task AddSubscriptionsAsync(CancellationToken cancellationToken)
    {
        await _espHandler.AddSubscriptions(cancellationToken);
        _espHandler.AddButtonPressedEvent(Input);
    }
    
    public void Input(int id)
    {
        _gamemode.ButtonPressed(id);
    }
}