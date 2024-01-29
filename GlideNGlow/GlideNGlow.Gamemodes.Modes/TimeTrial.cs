using System.Diagnostics;
using System.Drawing;
using GlideNGlow.Common.Extensions;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Gamemodes.Models.Abstractions;
using GlideNGlow.Gamemodes.Modes.Enums;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Models;
using GlideNGlow.Socket.Abstractions;

namespace GlideNGlow.Gamemodes.Modes;

public class TimeTrial : Gamemode
{
    private readonly Stopwatch _timeStarted = new();
    private int _startedButtonId =-1;
    private GameState _gameState;
    private readonly MeasurementLineRenderObject _countdownLight = new(0, 1, Color.Red);
    private TimeSpan _timeElapsed;
    private float _countdownTime = 5;
    private int _countdownStep;
    
    public TimeTrial(LightButtonHandler lightButtonHandler, AppSettings appSettings, ISocketWrapper socketWrapper) : base(lightButtonHandler, appSettings, socketWrapper)
    {
    }

    public override void Initialize(CancellationToken cancellationToken)
    {
        RenderObjects.Add(_countdownLight);
        _countdownLight.SetVisibility(false);
        _gameState = GameState.WaitingForStart;
        LightButtonHandler.SetAllRgb(Color.White, cancellationToken).GetAwaiter().GetResult();
    }

    public override void Stop()
    {
    }

    public override async Task UpdateAsync(TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        switch (_gameState)
        {
            case GameState.Countdown:
                await UpdateCountdownAsync(timeSpan);
                break;
            case GameState.Ending:
                await UpdateEndingAsync(timeSpan, cancellationToken);
                break;
            case GameState.WaitingForStart:
            case GameState.Running:
            case GameState.Error:
            default:
                break;
        }
    }
    
    private async Task UpdateEndingAsync(TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        _timeElapsed += timeSpan;
        if (_timeElapsed.TotalSeconds() >= 0)
        {
            _gameState = GameState.WaitingForStart;
            await LightButtonHandler.SetAllRgb(Color.White, cancellationToken);
        }
    }
    
    private async Task UpdateCountdownAsync(TimeSpan timeSpan)
    {
        _timeElapsed += timeSpan;
        switch (_countdownStep)
        {
            case 0 :
                if(_timeElapsed.TotalSeconds() >= -_countdownTime)
                {
                    _countdownStep = 1;
                    _countdownLight.SetColor(Color.Red);
                    await LightButtonHandler.SetRgb(_startedButtonId, Color.Red,new CancellationToken());
                }
                break;
            case 1 : 
                if(_timeElapsed.TotalSeconds() >= -_countdownTime/3*2)
                {
                    _countdownStep = 2;
                    Color color = Color.FromArgb(255, 85, 0);
                    _countdownLight.SetColor(color);
                    await LightButtonHandler.SetRgb(_startedButtonId, color ,new CancellationToken(),1);
                }
                break;
            case 2 :
                if(_timeElapsed.TotalSeconds() >= -_countdownTime/3)
                {
                    _countdownStep = 3;
                    _countdownLight.SetColor(Color.Yellow);
                    await LightButtonHandler.SetRgb(_startedButtonId, Color.Yellow,new CancellationToken(),1);
                }
                break;
            case 3 :
                if (_timeElapsed.TotalSeconds() >= 0)
                {
                    _countdownStep = 4;
                    _countdownLight.SetColor(Color.Green);
                    await LightButtonHandler.SetRgb(_startedButtonId, Color.Green,new CancellationToken(),0);
            
                    _gameState = GameState.Running;
                    _timeStarted.Start();
                }

                break;
        }
        
    }

    public override async Task ButtonPressed(int id, CancellationToken cancellationToken)
    {
        switch (_gameState)
        {
            case GameState.WaitingForStart:
                _startedButtonId = id;
                _gameState = GameState.Countdown;
                _countdownLight.SetVisibility(true);
                _timeElapsed = TimeSpan.FromSeconds(-_countdownTime);
                await LightButtonHandler.SetAllRgb(Color.Black, cancellationToken);
                //await LightButtonHandler.SetRgb(_startedButtonId, Color.Red,cancellationToken,2);
                
                _countdownStep = 0;
                var startDistance = AppSettings.Buttons[_startedButtonId].DistanceFromStart ?? 0;
                _countdownLight.SetStart(startDistance -5);
                _countdownLight.SetEnd(startDistance +5);
                break;
            case GameState.Countdown:
                break;
            case GameState.Running:
                if(_startedButtonId != id) return;
                _timeStarted.Stop();
                _countdownLight.SetVisibility(false);
                //await SocketWrapper.PublishUpdateScore(0, _timeStarted.ElapsedMilliseconds.ToString());
                await LightButtonHandler.SetRgb(_startedButtonId,Color.Black,cancellationToken);
                //await LightButtonHandler.SetRgb(AppSettings.Buttons[_startedButtonId].MacAddress, Color.Black,cancellationToken);
                _gameState = GameState.Ending;
                _timeElapsed = TimeSpan.FromSeconds(-_countdownTime);
                await SocketWrapper.PublishNewScores(TimeSpan.FromMilliseconds(_timeStarted.ElapsedMilliseconds).ToString(@"mm\:ss\.fff"));
                _timeStarted.Reset();
                break;
            case GameState.Ending:
            case GameState.Error:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}