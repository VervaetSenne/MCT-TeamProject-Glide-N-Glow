using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;

namespace GlideNGlow.Gamemodes.Models;

public class GamemodeHandler
{
    Gamemode _gamemode;
    private readonly LightRenderer _lightRenderer;
    private readonly EspHandler _espHandler;

    GamemodeHandler(LightRenderer lightRenderer, EspHandler espHandler)
    {
        _lightRenderer = lightRenderer;
        _espHandler = espHandler;
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

        await _lightRenderer.Update();
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