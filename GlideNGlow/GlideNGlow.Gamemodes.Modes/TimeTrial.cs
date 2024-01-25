using System.Diagnostics;
using System.Drawing;
using GlideNGlow.Common.Extensions;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Gamemodes.Models.Abstractions;
using GlideNGlow.Gamemodes.Modes.Enums;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Models;

namespace GlideNGlow.Gamemodes.Modes;

public class TimeTrial : Gamemode
{
    private Stopwatch _timeStarted = new();
    private int _startedButtonId =-1;
    private GameState _gameState;
    private readonly MeasurementLineRenderObject _countdownLight = new(0, 1, Color.Red);
    private float _timeElapsed = 0;
    private float _countdownTime = 3;
    private int _countdownStep = 0;
    
    public TimeTrial(LightButtonHandler lightButtonHandler, AppSettings appSettings) : base(lightButtonHandler, appSettings)
    {
    }

    public override void Initialize()
    {
        RenderObjects.Add(_countdownLight);
        _countdownLight.SetVisibility(false);
        _gameState = GameState.WaitingForStart;
    }

    public override void Stop()
    {
    }

    public override async Task UpdateAsync(TimeSpan timeSpan)
    {
        switch (_gameState)
        {
            case GameState.Countdown:
                await UpdateCountdownAsync(timeSpan);
                break;
            case GameState.WaitingForStart:
            case GameState.Running:
            case GameState.Ending:
                break;
            case GameState.Error:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private async Task UpdateCountdownAsync(TimeSpan timeSpan)
    {
        //TODO: creating new cancelation tokens instead of passing them
        _timeElapsed += timeSpan.TotalSeconds();
        if (_timeElapsed < -_countdownTime/3*2)
        {
            if (_countdownStep == 1) return;
            _countdownStep = 1;
            _countdownLight.SetColor(Color.Red);
            await LightButtonHandler.SetRgb(AppSettings.Buttons[_startedButtonId].MacAddress, Color.Red,new CancellationToken());
        }
        else if (_timeElapsed < -_countdownTime/3)
        {
            if (_countdownStep == 2) return;
            _countdownTime = 2;
            _countdownLight.SetColor(Color.Orange);
            await LightButtonHandler.SetRgb(AppSettings.Buttons[_startedButtonId].MacAddress, Color.Orange,new CancellationToken());
        }
        else if (_timeElapsed < 0)
        {
            if (_countdownStep == 3) return;
            _countdownStep= 3;
            _countdownLight.SetColor(Color.Yellow);
            await LightButtonHandler.SetRgb(AppSettings.Buttons[_startedButtonId].MacAddress, Color.Yellow,new CancellationToken());
        }
        else if (_timeElapsed >= 0)
        {
            _countdownStep = 4;
            _countdownLight.SetColor(Color.Green);
            await LightButtonHandler.SetRgb(AppSettings.Buttons[_startedButtonId].MacAddress, Color.Green,new CancellationToken());
            _gameState = GameState.Running;
            _timeStarted.Start();
        }
    }

    public override async Task ButtonPressed(int id)
    {
        switch (_gameState)
        {
            case GameState.WaitingForStart:
                _startedButtonId = id;
                _gameState = GameState.Countdown;
                _countdownLight.SetVisibility(true);
                _timeElapsed = -_countdownTime;
                
                _countdownStep = 0;
                var startDistance = AppSettings.Buttons[_startedButtonId].DistanceFromStart ?? 0;
                _countdownLight.SetStart(startDistance -2);
                _countdownLight.SetEnd(startDistance +2);
                break;
            case GameState.Countdown:
                break;
            case GameState.Running:
                if(_startedButtonId != id) return;
                _timeStarted.Stop();
                _countdownLight.SetVisibility(false);
                //TODO remove newly created cancelation token
                await LightButtonHandler.SetRgb(AppSettings.Buttons[_startedButtonId].MacAddress, Color.Black,new CancellationToken());
                _gameState = GameState.Ending;
                break;
            case GameState.Ending:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}