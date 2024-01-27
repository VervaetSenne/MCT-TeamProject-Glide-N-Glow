using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Gamemodes.Models;
using GlideNGlow.Gamemodes.Models.Abstractions;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Handlers;
using GlideNGlow.Socket;
using GlideNGlow.Socket.Abstractions;
using Microsoft.Extensions.Options;

namespace GlideNGlow.Gamemodes.Handlers;

public class GamemodeHandler
{
    private readonly IGameService _gameService;
    private readonly LightRenderer _lightRenderer;
    private readonly LightButtonHandler _lightButtonHandler;
    private readonly ISocketWrapper _socketWrapper;
    private readonly ScoreHandler _scoreHandler;

    private TaskCompletionSource _completionSource;
    private GamemodeData? _currentGamemode;

    public GamemodeHandler(IOptionsMonitor<AppSettings> appSettings, IGameService gameService,
        LightRenderer lightRenderer, LightButtonHandler lightButtonHandler, ISocketWrapper socketWrapper,
        ScoreHandler scoreHandler)
    {
        _gameService = gameService;
        _lightRenderer = lightRenderer;
        _lightButtonHandler = lightButtonHandler;
        _socketWrapper = socketWrapper;
        _scoreHandler = scoreHandler;
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
        if (gameType is null || !gameType.IsAssignableTo(typeof(IGamemode)))
            throw new ArgumentNullException(nameof(gameType), "Database was initialized wrongly!");

        IGamemode gamemode;
        if (gameType.BaseType?.IsGenericType ?? false) // gamemode requires settings
        {
            var constructorInfo = gameType.GetConstructor(new[] { typeof(LightButtonHandler), typeof(AppSettings), typeof(ISocketWrapper), typeof(string) });
            if (constructorInfo is null) throw new Exception("Every gamemode requires this constructor!");
            
            gamemode = (IGamemode)constructorInfo.Invoke(new object?[] {_lightButtonHandler, appSettings, _socketWrapper, appSettings.CurrentSettings});
        }
        else
        {
            var constructorInfo = gameType.GetConstructor(new[] { typeof(LightButtonHandler), typeof(AppSettings), typeof(ISocketWrapper) });
            if (constructorInfo is null) throw new Exception("Every gamemode requires this constructor!");
            
            gamemode = (IGamemode)constructorInfo.Invoke(new object?[] {_lightButtonHandler, appSettings, _socketWrapper});
        }

        _currentGamemode = new GamemodeData
        {
            Game = game,
            Gamemode = gamemode
        };
        _scoreHandler.RemoveScores(game.Id);
        
        _completionSource.SetResult();
        _completionSource = new TaskCompletionSource();
    }
    
    public async Task TryInitializeAsync(CancellationToken cancellationToken)
    {
        cancellationToken.Register(() => _completionSource.SetCanceled(cancellationToken));
        if (_currentGamemode is null)
            await _completionSource.Task;
        else
            return;

        if (_currentGamemode is null)
            return;

        if (cancellationToken.IsCancellationRequested)
            return;
        
        _currentGamemode!.Gamemode.Initialize(cancellationToken);
        await _lightButtonHandler.AddSubscriptions(cancellationToken);
        _lightButtonHandler.AddButtonPressedEvent(i => Input(i, cancellationToken));
    }
    
    public async Task UpdateAsync(TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        if (_currentGamemode is null)
            return;
        
        await _currentGamemode.Gamemode.UpdateAsync(timeSpan, cancellationToken);
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
        await _lightButtonHandler.RemoveSubscriptions(cancellationToken);
        _currentGamemode = null;
    }
    
    public async Task Input(int id, CancellationToken cancellationToken)
    {
        if (_currentGamemode is not null) await _currentGamemode.Gamemode.ButtonPressed(id, cancellationToken);
    }
}