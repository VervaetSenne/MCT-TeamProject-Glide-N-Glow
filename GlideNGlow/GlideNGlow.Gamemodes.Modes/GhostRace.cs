using System.Drawing;
using GlideNGlow.Common.Extensions;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Gamemodes.Models.Abstractions;
using GlideNGlow.Gamemodes.Modes.Enums;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Models;
using GlideNGlow.Rendering.Models.Abstractions;

namespace GlideNGlow.Gamemodes.Modes;

public class GhostRace : Gamemode, IGamemode
{
    private readonly AppSettings _appsettings;
    private readonly float _distanceCm;
    private readonly float _timeLimit;
    private readonly MeasurementLineRenderObject _ghostLight = new(0, 0, Color.Red);
    private readonly MeasurementLineRenderObject _countdownLight = new(0, -1, Color.White);
    
    private float _distancePerSecond;
    private GameState _gameState;
    private int _startedButtonId;
    private float _timeElapsed;
    
    public GhostRace(EspHandler espHandler, AppSettings appsettings, float timeLimit) : base(espHandler, appsettings)
    {
        _appsettings = appsettings;
        _distanceCm = appsettings.Strips.Sum(strip => strip.Length + strip.DistanceFromLast);
        _timeLimit = timeLimit;
    }

    public void Initialize()
    {
        _distancePerSecond = _distanceCm / _timeLimit;
        _gameState = GameState.WaitingForStart;
    }

    public void Stop()
    {
    }

    public Task UpdateAsync(TimeSpan timeSpan)
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
            default:
                throw new ArgumentOutOfRangeException();
        }
        return Task.CompletedTask;
    }

    public void UpdateCountdown(TimeSpan timeSpan)
    {
        var cdLightOn = _timeElapsed % 1 > -0.5;
        _countdownLight.SetColor(cdLightOn ? Color.White : Color.Black);
        if (_timeElapsed > 0)
        {
            RenderObjects.Remove(_countdownLight);
            _gameState = GameState.Running;
            _timeElapsed = 0;
        }
    }
    
    public void UpdateRunning(TimeSpan timeSpan)
    {
        //calculate the distance to move
        var distanceToMove = _distancePerSecond * timeSpan.TotalSeconds();
        _ghostLight.Move(distanceToMove);
        if (_timeElapsed > _timeLimit)
        {
            RenderObjects.Remove(_ghostLight);
            _gameState = GameState.WaitingForStart;
            _timeElapsed = 0;
        }
    }

    public List<RenderObject> GetRenderObjects()
    {
        return RenderObjects;
    }

    public Task ButtonPressed(int id)
    {
        if (_gameState != GameState.WaitingForStart) return Task.CompletedTask;
        
        _timeElapsed = -3;
        _startedButtonId = id;
            
        RenderObjects.Add(_countdownLight);
            
        var startDistance = _appsettings.Buttons[_startedButtonId].DistanceFromStart ?? 0;
        _ghostLight.SetStart(startDistance);
        _ghostLight.SetEnd((float)(startDistance + 0.2));
        RenderObjects.Add(_ghostLight);
            
        _gameState = GameState.Countdown;
        return Task.CompletedTask;
    }
}