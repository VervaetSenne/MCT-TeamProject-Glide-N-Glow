using System.Drawing;
using GlideNGlow.Mqqt.Models;
using GlideNGlow.Rendering.Models;

namespace GlideNGlow.Gamemodes.Models;

public class GhostRace : Gamemode
{
    
    List<RenderObject> _renderObjects = new();
    private float distanceCm = 100, timeLimit = 10;
    private float _distancePerSecond;
    private GameState _gameState;
    private int _startedButtonId;
    
    enum GameState
    {
        WaitingForStart,
        Countdown,
        Running,
    }
    
    private float _timeElapsed;
    GhostRace(EspHandler espHandler, float distanceCm, float timeLimit)
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
        if (_timeElapsed > 0)
        {
            _gameState = GameState.Running;
        }
        _timeElapsed = 0;
    }
    
    public void UpdateRunning(float deltaSeconds)
    {
        _timeElapsed += deltaSeconds;
        if (_timeElapsed > timeLimit)
        {
            _gameState = GameState.WaitingForStart;
        }
        _timeElapsed = 0;
    }

    public override List<RenderObject> GetRenderObjects()
    {
        throw new NotImplementedException();
    }

    public override Task Input(int id)
    {
        if (_gameState == GameState.WaitingForStart)
        {
            _timeElapsed = -3;
            _startedButtonId = id;
            _gameState = GameState.Countdown;
        }
        throw new NotImplementedException();
    }
}