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
    
    public GhostRace(EspHandler espHandler, AppSettings appsettings, string settingsJson) : base(espHandler, appsettings, settingsJson)
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
    }

    public override Task UpdateAsync(TimeSpan timeSpan)
    {
        _timeElapsed += timeSpan.TotalSeconds();
        switch (_gameState)
        {
            case GameState.WaitingForStart:
                break;
            case GameState.Countdown:
                UpdateCountdown(timeSpan);
                break;
            case GameState.Running:
                UpdateRunning(timeSpan);
                break;
            case GameState.Ending:
                UpdateEnding(timeSpan);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return Task.CompletedTask;
    }

    private void UpdateCountdown(TimeSpan timeSpan)
    {
        var cdLightOn = _timeElapsed % 1 > -0.5;
        _countdownLight.SetVisibility(cdLightOn);
        if (_timeElapsed > 0)
        {
            _countdownLight.SetVisibility(false);
            _gameState = GameState.Running;
            _timeElapsed = 0;
        }
    }

    private void UpdateRunning(TimeSpan timeSpan)
    {
        //calculate the distance to move
        var distanceToMove = _distancePerSecond * timeSpan.TotalSeconds();
        _ghostLight.Move(distanceToMove);
        if (_timeElapsed > Settings.TimeLimit)
        {
            _countdownLight.SetVisibility(true);
            _gameState = GameState.Ending;
            _timeElapsed = -3;
            ForceRenderUpdate = true;
        }
    }
    
    private void UpdateEnding(TimeSpan timeSpan)
    {
        if (_timeElapsed > 0)
        {
            _ghostLight.SetVisibility(false);
            _countdownLight.SetVisibility(false);
            _gameState = GameState.WaitingForStart;
            _timeElapsed = 0;
        }
    }

    public override Task ButtonPressed(int id)
    {
        if (_gameState != GameState.WaitingForStart) return Task.CompletedTask;
        
        _timeElapsed = -3;
        _startedButtonId = id;
        
            
        var startDistance = AppSettings.Buttons[_startedButtonId].DistanceFromStart ?? 0;
        _ghostLight.SetStart(startDistance);
        _ghostLight.SetEnd(startDistance + _distancePerSecond * 0.3f);
        _ghostLight.SetVisibility(true);
            
        _countdownLight.SetStart(startDistance - _distancePerSecond);
        _countdownLight.SetEnd(startDistance + _distancePerSecond);
        _gameState = GameState.Countdown;
        return Task.CompletedTask;
    }
}