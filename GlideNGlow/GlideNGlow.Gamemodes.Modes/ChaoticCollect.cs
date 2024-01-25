using System.Drawing;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Gamemodes.Models.Abstractions;
using GlideNGlow.Gamemodes.Modes.Enums;
using GlideNGlow.Gamemodes.Modes.Settings;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Models;
using GlideNGlow.Socket.Abstractions;

namespace GlideNGlow.Gamemodes.Modes;

public class ChaoticCollect : Gamemode<ChaoticCollectSettings>
{
    private readonly Dictionary<int, Color> _playerColors;
    private readonly Dictionary<int,float> _buttonIds = new();
    private int _playerAmount;
    private TimeSpan _timeElapsed;
    private GameState _gameState = GameState.WaitingForStart;
    private readonly List<int> _buttonAssignments = new();
    private readonly Random _random = new();
    private readonly List<int> _scores = new();
    private readonly float _countdownTime = 3f;
    private readonly Dictionary<int,MeasurementLineRenderObject> _displayLines = new();
    private float _buttonWidth = 25f;
    private int _countdownStep;
    
    public ChaoticCollect(LightButtonHandler lightButtonHandler, AppSettings appSettings, string settingsJson,  ISocketWrapper socketWrapper) : base(lightButtonHandler, appSettings, settingsJson, socketWrapper)
    {
        //0 being nothing
        _playerColors = new()
        {
            { 0, Color.Black },
            { 1, Color.Red },
            { 2, Color.Blue },
            { 3, Color.Green },
            { 4, Color.Purple },
            { 5, Color.Yellow },
            { 6, Color.Cyan },
            { 7, Color.Orange },
            { 8, Color.Pink }
        };
    }

    public override void Initialize(CancellationToken cancellationToken)
    {
        foreach (var lightButtonsValue in LightButtonHandler.LightButtons.Values)
        {
            if (lightButtonsValue.ButtonNumber != -1)
            {
                var buttonNumber = lightButtonsValue.ButtonNumber ?? -1;
                var distance = lightButtonsValue.DistanceFromStart ?? -1;
                _buttonIds.Add(buttonNumber, distance);
                var mlro = new MeasurementLineRenderObject(distance - _buttonWidth / 2, distance + _buttonWidth / 2,
                    Color.Black);
                RenderObjects.Add(mlro);
                _displayLines.Add(buttonNumber,mlro);
            }
        }

        if (_buttonIds.Count < 2)
        {
            ChangeStateAsync(GameState.Error, cancellationToken).GetAwaiter().GetResult();
            return;
        }

        _playerAmount = int.Min(int.Min(Settings.PlayerAmount, _buttonIds.Count - 1),_playerColors.Count - 1);
        
        _buttonAssignments.AddRange(Enumerable.Repeat(0, _buttonIds.Count));
        
        
        for (var i = 0; i < _playerAmount; i++)
        {
            _scores.Add(0);
        }
        ChangeStateAsync(GameState.WaitingForStart, cancellationToken).GetAwaiter().GetResult();
    }

    public override void Stop()
    {
    }

    public override async Task UpdateAsync(TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        _timeElapsed += timeSpan;
        switch (_gameState)
        {
            case GameState.WaitingForStart:
                break;
            case GameState.Countdown:
                await UpdateCountdown(cancellationToken);
                break;
            case GameState.Running:
                await UpdateRunning(cancellationToken);
                break;
            case GameState.Ending:
                break;
            case GameState.Error:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    
    private async Task UpdateCountdown(CancellationToken cancellationToken)
    {
        switch (_countdownStep)
        {
            case 0 :
                if(_timeElapsed.TotalSeconds >= -_countdownTime/3*2)
                {
                    _countdownStep = 1;
                    await SetAllColor(Color.Red, default);
                }
                break;
            case 1 : 
                if(_timeElapsed.TotalSeconds >= -_countdownTime/3*2)
                {
                    _countdownStep = 2;
                    await SetAllColor(Color.Orange, default);
                }
                break;
            case 2 :
                if(_timeElapsed.TotalSeconds >= -_countdownTime/3*2)
                {
                    _countdownStep = 3;
                    await SetAllColor(Color.Yellow, default);
                }
                break;
        }
        
        if (_timeElapsed.TotalSeconds >= 0)
        {
            await ChangeStateAsync(GameState.Running, cancellationToken);
        }
        
    }

    private async Task UpdateRunning(CancellationToken cancellationToken)
    {
        if(_timeElapsed.TotalSeconds >= Settings.TimeLimit)
        {
            await ChangeStateAsync(GameState.Ending, cancellationToken);
        }
    }

    private async Task ChangeStateAsync(GameState newState, CancellationToken cancellationToken)
    {
        if (_buttonIds.Count < 2)
        {
            newState = GameState.Error;
        }
        
        switch(newState)
        {
            case GameState.WaitingForStart:
                _scores.ForEach(x => x = 0);
                await LightButtonHandler.SetAllRgb(Color.White, default);
                SetAllColor(Color.White, new CancellationToken()).Wait();
                break;
            case GameState.Countdown:
                _countdownStep = 0;
                _timeElapsed = TimeSpan.FromSeconds(-_countdownTime);
                //await LightButtonHandler.SetAllRgb(Color.White, new CancellationToken());
                break;
            case GameState.Running:
                await RunningStart(cancellationToken);
                break;
            case GameState.Ending:
                break;
            case GameState.Error:
                await LightButtonHandler.SetAllRgb(Color.Orange, default);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        _gameState = newState;
    }

    private async Task RunningStart(CancellationToken cancellationToken)
    {
        //for each player, set an element in _buttonAssignements (that is 0) to its color
        
        for (var i = 1; i <= _playerAmount; i++)
        {
            await AssignColorToEmpty(i, cancellationToken);
        }
        //find the _buttonAssignments which is set to -1 and change that to 0
        _buttonAssignments[_buttonAssignments.FindIndex(x => x == -1)] = 0;
    }

    private async Task AssignColorToEmpty(int playerId, CancellationToken cancellationToken)
    {
        //get a random button id
        var randomButtonId = _random.Next(1, _buttonIds.Count);
        //check if that button is already assigned
        while (_buttonAssignments[randomButtonId] != 0)
        {
            //if it is, go to the next id (or the first if we are at the end)
            randomButtonId = randomButtonId == _buttonIds.Count ? 1 : randomButtonId + 1;
        }
        //assign the button to the player
        _buttonAssignments[randomButtonId] = playerId;
        //set the button to the player's color
        await SetColor(randomButtonId, _playerColors[playerId], cancellationToken);
    }
    
    private async Task RemoveAssignment(int buttonId, CancellationToken cancellationToken)
    {
        await SetColor(buttonId, Color.Black, cancellationToken);
        //set the button assignment to 0
        _buttonAssignments[buttonId] = 0;
    }
    
    private async Task SetAllColor(Color color, CancellationToken cancellationToken)
    {
        foreach (var buttonId in _buttonAssignments)
        {
            await SetColor(buttonId, color, cancellationToken);
        }
    }
    

    private async Task SetColor(int buttonId, Color color, CancellationToken cancellationToken)
    {
        await LightButtonHandler.SetRgb(buttonId, color, cancellationToken);
        _displayLines[buttonId].SetColor(color);
    }
    

    private async Task ButtonPressedRun(int id, CancellationToken cancellationToken)
    {
        //if the button is assigned to a player
        if (_buttonAssignments[id] < 1) return; //return if the button is not assigned to a player
        
        //get the player id
        var playerId = _buttonAssignments[id];
        
        //remove the assignment
        await AssignColorToEmpty(playerId, cancellationToken);
            
        //assign a new button to the player
        await RemoveAssignment(id, cancellationToken);

        //add a point to the player
        _scores[playerId-1]++;
        
    }

    public override async Task ButtonPressed(int id, CancellationToken cancellationToken)
    {
        if (GameState.WaitingForStart == _gameState)
        {
            await ChangeStateAsync(GameState.Countdown, cancellationToken);
            _buttonAssignments[id] = -1;
            return;
        }
        
        if(GameState.Running == _gameState)
        {
            await ButtonPressedRun(id, cancellationToken);
        }
        
    }
}