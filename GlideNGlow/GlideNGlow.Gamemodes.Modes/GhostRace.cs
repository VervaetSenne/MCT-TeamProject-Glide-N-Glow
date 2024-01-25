using System.Drawing;
using GlideNGlow.Common.Extensions;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Gamemodes.Models.Abstractions;
using GlideNGlow.Gamemodes.Modes.Enums;
using GlideNGlow.Gamemodes.Modes.Settings;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Models;

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
    
    public GhostRace(LightButtonHandler lightButtonHandler, AppSettings appsettings, string settingsJson) : base(lightButtonHandler, appsettings, settingsJson)
    {
        _distanceCm = appsettings.Strips.Sum(strip => strip.Length + strip.DistanceFromLast);
    }

    public override void Initialize()
    {
        RenderObjects.Add(_countdownLight);
        RenderObjects.Add(_ghostLight);
        _ghostLight.SetVisibility(false);
        _countdownLight.SetVisibility(false);
        _distancePerSecond = _distanceCm / Settings.TimeLimit;
        _gameState = GameState.WaitingForStart;
    }

    public override void Stop()
    {
        LightButtonHandler.SetAllRgb(Color.Black, new CancellationToken()).Wait();
    }

    public override async Task UpdateAsync(TimeSpan timeSpan)
    {
        _timeElapsed += timeSpan.TotalSeconds();
        switch (_gameState)
        {
            case GameState.WaitingForStart:
                break;
            case GameState.Countdown:
                await UpdateCountdown();
                break;
            case GameState.Running:
                await UpdateRunning(timeSpan);
                break;
            case GameState.Ending:
                await UpdateEnding();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task UpdateCountdown()
    {
        var cdLightOn = _timeElapsed % 1 > -0.5;
        _countdownLight.SetVisibility(cdLightOn);
        if (_timeElapsed > 0)
        {
            await SwitchGameState(GameState.Running);
        }
    }

    private async Task UpdateRunning(TimeSpan timeSpan)
    {
        //calculate the distance to move
        var distanceToMove = _distancePerSecond * timeSpan.TotalSeconds();
        _ghostLight.Move(distanceToMove);
        if (_timeElapsed > Settings.TimeLimit)
        {
            await SwitchGameState(GameState.Ending);
        }
    }

    private async Task SwitchGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.WaitingForStart:
                _ghostLight.SetVisibility(false);
                _countdownLight.SetVisibility(false);
                await LightButtonHandler.SetAllRgb(Color.White, new CancellationToken());
                _timeElapsed = 0;
                break;
            case GameState.Countdown:
                _timeElapsed = -3;
                _ghostLight.SetVisibility(true);
                _countdownLight.SetVisibility(false);
                await LightButtonHandler.SetAllRgb(Color.Black, new CancellationToken());
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
    
    private async Task UpdateEnding()
    {
        if (_timeElapsed > 0)
        {
            await SwitchGameState(GameState.WaitingForStart);
        }
    }

    public override async Task ButtonPressed(int id)
    {
        if (_gameState != GameState.WaitingForStart) return;
        
        _startedButtonId = id;
        await SwitchGameState(GameState.Countdown);
        
        
    }
}