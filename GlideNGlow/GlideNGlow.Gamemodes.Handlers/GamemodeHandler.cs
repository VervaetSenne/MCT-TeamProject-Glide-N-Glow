using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Gamemodes.Models;
using GlideNGlow.Gamemodes.Models.Abstractions;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Handlers;
using Microsoft.Extensions.Options;

namespace GlideNGlow.Gamemodes.Handlers;

public class GamemodeHandler
{
    private readonly IOptionsMonitor<GamemodeSettings> _gamemodeSettings;
    private readonly IGameService _gameService;
    private readonly LightRenderer _lightRenderer;
    private readonly EspHandler _espHandler;
    private readonly TaskCompletionSource _completionSource;
    
    private GamemodeData? _currentGamemode;

    public GamemodeHandler(IOptionsMonitor<AppSettings> appSettings, IOptionsMonitor<GamemodeSettings> gamemodeSettings,
        IGameService gameService, LightRenderer lightRenderer, EspHandler espHandler)
    {
        _gamemodeSettings = gamemodeSettings;
        _gameService = gameService;
        _lightRenderer = lightRenderer;
        _espHandler = espHandler;
        _completionSource = new TaskCompletionSource();
        
        appSettings.OnChange(AppSettingsChanged);
    }

    private void AppSettingsChanged(AppSettings appSettings)
    {
        if (_currentGamemode is not null && _currentGamemode.Game.Id == appSettings.CurrentGamemode ||
            _currentGamemode is null && appSettings.CurrentGamemode is null)
            return;
        
        var game = _gameService.FindByIdAsync(appSettings.CurrentGamemode).GetAwaiter().GetResult();
        if (game is null)
        {
            StopGameAsync(default).GetAwaiter().GetResult();
            return;
        }

        var gameType = Type.GetType(game.AssemblyName);
        if (gameType is null || gameType.IsAssignableTo(typeof(IGamemode)))
            throw new ArgumentNullException(nameof(gameType), "Database was initialized wrongly!");

        IGamemode gamemode;
        if (gameType.BaseType?.IsGenericType ?? false) // gamemode requires settings
        {
            var constructorInfo = gameType.GetConstructor(new[] { typeof(EspHandler), typeof(AppSettings), typeof(string) });
            if (constructorInfo is null) throw new Exception("Every gamemode requires this constructor!");
            
            gamemode = (IGamemode)constructorInfo.Invoke(new object?[] {_espHandler, appSettings, _gamemodeSettings.CurrentValue.CurrentSettings});
        }
        else
        {
            var constructorInfo = gameType.GetConstructor(new[] { typeof(EspHandler), typeof(AppSettings) });
            if (constructorInfo is null) throw new Exception("Every gamemode requires this constructor!");
            
            gamemode = (IGamemode)constructorInfo.Invoke(new object?[] {_espHandler, appSettings});
        }

        _currentGamemode = new GamemodeData
        {
            Game = game,
            Gamemode = gamemode
        };
        _completionSource.SetResult();
    }
    
    public async Task TryInitializeAsync(CancellationToken cancellationToken)
    {
        cancellationToken.Register(() => _completionSource.SetCanceled(cancellationToken));
        if (_currentGamemode is null)
            await _completionSource.Task;
        else
            return;

        if (cancellationToken.IsCancellationRequested)
            return;

        if (_currentGamemode is null)
            throw new Exception();
        
        _currentGamemode!.Gamemode.Initialize();
        await _espHandler.AddSubscriptions(cancellationToken);
        _espHandler.AddButtonPressedEvent(Input);
    }
    
    public async Task UpdateAsync(TimeSpan timeSpan)
    {
        if (_currentGamemode is null)
            return;
        
        await _currentGamemode.Gamemode.UpdateAsync(timeSpan);
    }
    
    public async Task RenderAsync(CancellationToken cancellationToken)
    {
        if (_currentGamemode is null)
            return;

        if (_lightRenderer.PixelAmount <= 0)
            return;

        if (_currentGamemode.Gamemode.ShouldForceRender())
        {
            _lightRenderer.MakeDirty();
        }
        _lightRenderer.Clear();
        foreach (var renderObject in _currentGamemode.Gamemode.GetRenderObjects())
        {
            renderObject.Render(_lightRenderer);
        }
        
        await _lightRenderer.ShowAsync(cancellationToken);
    }

    private async Task StopGameAsync(CancellationToken cancellationToken)
    {
        if (_currentGamemode is null)
            return;
        
        _currentGamemode.Gamemode.Stop();
        await _espHandler.RemoveSubscriptions(cancellationToken);
    }
    
    public void Input(int id)
    {
        _currentGamemode?.Gamemode.ButtonPressed(id);
    }
}