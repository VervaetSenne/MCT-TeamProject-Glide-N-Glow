using System.Drawing;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Gamemodes.Models.Enums;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;

namespace GlideNGlow.Gamemodes.Models;

public class GhostRace : Gamemode
{
    private readonly AppSettings _appsettings;
    private readonly float _distanceCm;
    private readonly float _timeLimit;
    private readonly List<RenderObject> _renderObjects = new();
    private readonly MeasurementLineRenderObject _ghostLight = new(0, 0, Color.Red);
    private readonly MeasurementLineRenderObject _countdownLight = new(0, -1, Color.White);
    
    private float _distancePerSecond;
    private GameState _gameState;
    private int _startedButtonId;
    private float _timeElapsed;
    
    public GhostRace(AppSettings appsettings, EspHandler espHandler, float distanceCm, float timeLimit) : base(espHandler)
    {
        EspHandler = espHandler;
        _appsettings = appsettings;
        _distanceCm = distanceCm;
        _timeLimit = timeLimit;
    }

    public override Task Start()
    {
        _distancePerSecond = _distanceCm / _timeLimit;
        _gameState = GameState.WaitingForStart;
        return Task.CompletedTask;
    }

    public override Task Update(float deltaSeconds)
    {
        _timeElapsed += deltaSeconds;
        switch (_gameState)
        {
            case GameState.WaitingForStart:
                break;
            case GameState.Countdown:
                UpdateCountdown(deltaSeconds);
                break;
            case GameState.Running:
                UpdateRunning(deltaSeconds);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return Task.CompletedTask;
    }

    public void UpdateCountdown(float deltaSeconds)
    {
        var cdLightOn = _timeElapsed % 1 > 0.5;
        _countdownLight.SetColor(cdLightOn ? Color.White : Color.Black);
        if (_timeElapsed > 0)
        {
            _renderObjects.Remove(_countdownLight);
            _gameState = GameState.Running;
            _timeElapsed = 0;
        }
    }
    
    public void UpdateRunning(float deltaSeconds)
    {
        //calculate the distance to move
        var distanceToMove = _distancePerSecond * deltaSeconds;
        _ghostLight.Move(distanceToMove);
        if (_timeElapsed > _timeLimit)
        {
            _renderObjects.Remove(_ghostLight);
            _gameState = GameState.WaitingForStart;
            _timeElapsed = 0;
        }
    }

    public override List<RenderObject> GetRenderObjects()
    {
        return _renderObjects;
    }

    public override Task Input(int id)
    {
        if (_gameState != GameState.WaitingForStart) return Task.CompletedTask;
        
        _timeElapsed = -3;
        _startedButtonId = id;
            
        _renderObjects.Add(_countdownLight);
            
        var startDistance = _appsettings.Buttons[_startedButtonId].DistanceFromStart ?? 0;
        _ghostLight.SetStart(startDistance-2);
        _ghostLight.SetEnd(startDistance + _distanceCm + 2);
        _renderObjects.Add(_ghostLight);
            
        _gameState = GameState.Countdown;
        return Task.CompletedTask;
    }
}