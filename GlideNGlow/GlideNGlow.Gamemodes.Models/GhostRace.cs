using System.Drawing;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;

namespace GlideNGlow.Gamemodes.Models;

public class GhostRace : Gamemode
{
    private float distanceCm = 100, timeLimit = 10;
    private float _distancePerSecond;
    private GameState _gameState;
    private int _startedButtonId;
    private List<RenderObject> _renderObjects = new List<RenderObject>();
    private MeasurementLineRenderObject _ghostLight = new MeasurementLineRenderObject(0, 0, Color.Red);
    private MeasurementLineRenderObject _countdownLight = new MeasurementLineRenderObject(0, -1, Color.White);
    
    enum GameState
    {
        WaitingForStart,
        Countdown,
        Running,
    }
    
    private float _timeElapsed;
    public GhostRace(EspHandler espHandler, float distanceCm, float timeLimit)
    {
        _espHandler = espHandler;
        this.distanceCm = distanceCm;
        this.timeLimit = timeLimit;
    }

    public override Task Start()
    {
        _distancePerSecond = distanceCm / timeLimit;
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
                break;
        }
        return Task.CompletedTask;
    }

    public void UpdateCountdown(float deltaSeconds)
    {
        _timeElapsed += deltaSeconds;
        bool cdLightOn = _timeElapsed % 1 > 0.5;
        _countdownLight.SetColor(cdLightOn ? Color.White : Color.Black);
        if (_timeElapsed > 0)
        {
            _renderObjects.Remove(_countdownLight);
            _gameState = GameState.Running;
        }
        
        _timeElapsed = 0;
    }
    
    public void UpdateRunning(float deltaSeconds)
    {
        _timeElapsed += deltaSeconds;
        //calculate the distance to move
        var distanceToMove = _distancePerSecond * deltaSeconds;
        _ghostLight.Move(distanceToMove);
        if (_timeElapsed > timeLimit)
        {
            _gameState = GameState.WaitingForStart;
        }
        _timeElapsed = 0;
    }

    public override List<RenderObject> GetRenderObjects()
    {
        return _renderObjects;
    }

    public override Task Input(int id)
    {
        if (_gameState == GameState.WaitingForStart)
        {
            _timeElapsed = -3;
            _startedButtonId = id;
            
            _renderObjects.Add(_countdownLight);
            
            var startDistance = _espHandler.GetAppSettings().Buttons[_startedButtonId].DistanceFromStart ?? 0;
            _ghostLight.SetStart(startDistance-2);
            _ghostLight.SetEnd(startDistance + distanceCm + 2);
            _renderObjects.Add(_ghostLight);
            
            _gameState = GameState.Countdown;
        }
        return Task.CompletedTask;
    }
}