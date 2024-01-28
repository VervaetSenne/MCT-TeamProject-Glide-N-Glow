using System.Drawing;
using GlideNGlow.Common.Extensions;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Gamemodes.Models.Abstractions;
using GlideNGlow.Gamemodes.Modes.Enums;
using GlideNGlow.Gamemodes.Modes.Settings;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Models;
using GlideNGlow.Socket.Abstractions;

namespace GlideNGlow.Gamemodes.Modes;

public class GhostRace : Gamemode<GhostRaceSetting>
{
    private readonly float _distanceCm;
    private readonly MeasurementLineRenderObject _ghostLight = new(0, 0, Color.Red);
    private readonly MeasurementLineRenderObject _countdownLight = new(0, -1, Color.White);
    
    private float _distancePerSecond;
    private GameState _gameState;
    private int _startedButtonId;
    private float _timeElapsed;
    
    public GhostRace(LightButtonHandler lightButtonHandler, AppSettings appsettings, ISocketWrapper socketWrapper, string settingsJson) : base(lightButtonHandler, appsettings, socketWrapper, settingsJson)
    {
        _distanceCm = appsettings.Strips.Sum(strip => strip.Length + strip.DistanceFromLast);
    }

    public override void Initialize(CancellationToken cancellationToken)
    {
        RenderObjects.Add(_countdownLight);
        RenderObjects.Add(_ghostLight);
        _ghostLight.SetVisibility(false);
        _countdownLight.SetVisibility(false);
        _distancePerSecond = _distanceCm / Settings.TimeLimit;
        // _gameState = GameState.WaitingForStart;
        SwitchGameState(GameState.WaitingForStart, cancellationToken).Wait();
    }

    public override void Stop()
    {
        LightButtonHandler.SetAllRgb(Color.Black, new CancellationToken()).Wait();
    }

    public override async Task UpdateAsync(TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        //extension from using GlideNGlow.Common.Extensions;
        _timeElapsed += timeSpan.TotalSeconds();
        switch (_gameState)
        {
            case GameState.WaitingForStart:
                break;
            case GameState.Countdown:
                await UpdateCountdown(cancellationToken);
                break;
            case GameState.Running:
                await UpdateRunning(timeSpan, cancellationToken);
                break;
            case GameState.Ending:
                await UpdateEnding(cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task UpdateCountdown(CancellationToken cancellationToken)
    {
        var cdLightOn = _timeElapsed % 1 > -0.5;
        _countdownLight.SetVisibility(cdLightOn);
        if (_timeElapsed > 0)
        {
            await SwitchGameState(GameState.Running, cancellationToken);
        }
    }

    private async Task UpdateRunning(TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        //calculate the distance to move
        var distanceToMove = _distancePerSecond * timeSpan.TotalSeconds();
        _ghostLight.Move(distanceToMove);
        if (_timeElapsed > Settings.TimeLimit)
        {
            await SwitchGameState(GameState.Ending, cancellationToken);
        }
    }

    private async Task SwitchGameState(GameState newState, CancellationToken cancellationToken)
    {
        switch (newState)
        {
            case GameState.WaitingForStart:
                _ghostLight.SetVisibility(false);
                _countdownLight.SetVisibility(false);
                await LightButtonHandler.SetAllRgb(Color.White, cancellationToken);
                _timeElapsed = 0;
                break;
            case GameState.Countdown:
                _timeElapsed = -3;
                _ghostLight.SetVisibility(true);
                _countdownLight.SetVisibility(false);
                await LightButtonHandler.SetAllRgb(Color.Black, cancellationToken);
                var startDistance = AppSettings.Buttons[_startedButtonId].DistanceFromStart ?? 0;
                _ghostLight.SetStart(startDistance);
                _ghostLight.SetEnd(startDistance + _distancePerSecond * 0.3f);
            
                _countdownLight.SetStart(startDistance - _distancePerSecond);
                _countdownLight.SetEnd(startDistance + _distancePerSecond);
                break;
            case GameState.Running:
                _ghostLight.SetVisibility(true);
                _countdownLight.SetVisibility(false);
                _gameState = GameState.Running;
                _timeElapsed = 0;
                break;
            case GameState.Ending:
                _ghostLight.SetVisibility(true);
                _countdownLight.SetVisibility(true);
                _timeElapsed = -3;
                ForceRenderUpdate = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        _gameState = newState;
    }
    
    private async Task UpdateEnding(CancellationToken cancellationToken)
    {
        if (_timeElapsed > 0)
        {
            await SwitchGameState(GameState.WaitingForStart, cancellationToken);
        }
    }

    public override async Task ButtonPressed(int id, CancellationToken cancellationToken)
    {
        if (_gameState != GameState.WaitingForStart) return;
        
        _startedButtonId = id;
        await SwitchGameState(GameState.Countdown, cancellationToken);
        
        
    }
}