using System.Drawing;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Gamemodes.Models.Abstractions;
using GlideNGlow.Mqqt.Handlers;
using GlideNGlow.Rendering.Models;
using GlideNGlow.Rendering.Models.Abstractions;
using GlideNGlow.Socket.Abstractions;
using GlideNGlow.Common.Extensions;

namespace GlideNGlow.Gamemodes.Modes;

public class CalibrationMode : Gamemode
{
    private readonly LightButtonHandler _lightButtonHandler;
    private readonly AppSettings _appSettings;
    private readonly ISocketWrapper _socketWrapper;
    
    //timer for updating render objects
    private float _timeElapsed;
    private float _updateInterval = 5f;
    
    private float _halfWidth = 2.5f;
    

    public CalibrationMode(LightButtonHandler lightButtonHandler, AppSettings appSettings, ISocketWrapper socketWrapper) : base(lightButtonHandler, appSettings, socketWrapper)
    {
        _lightButtonHandler = lightButtonHandler;
        _appSettings = appSettings;
        _socketWrapper = socketWrapper;
    }

    public override void Initialize(CancellationToken cancellationToken)
    {
        UpdateSettingsAsync(cancellationToken).GetAwaiter().GetResult();
    }

    private async Task UpdateSettingsAsync(CancellationToken cancellationToken)
    {
        RecalculateMeasurementLines();
        await RecalculateButtonLines(cancellationToken);
    }

    private async Task RecalculateButtonLines(CancellationToken cancellationToken)
    {
        foreach (var lightButtonsValue in _lightButtonHandler.LightButtons.Values)
        {
            if (lightButtonsValue.ButtonNumber != -1)
            {
                var buttonNumber = lightButtonsValue.ButtonNumber ?? -1;
                var distance = lightButtonsValue.DistanceFromStart ?? -1;
                string colorHex = lightButtonsValue.MacAddress.MacToHex();
                //convert color Hex to Color
                var color = ColorTranslator.FromHtml("0x" + colorHex);
                var mlro = new MeasurementLineRenderObject(distance - _halfWidth, distance + _halfWidth,
                    color);
                await LightButtonHandler.SetRgb(buttonNumber, color, cancellationToken);
                RenderObjects.Add(mlro);
            }
        }
    }

    private void RecalculateMeasurementLines()
    {
        
        //create interval measurement lines
        var interval = 50f;
        var distance = 0f;
        var sum = _appSettings.Strips.Sum(strip => strip.Length + strip.DistanceFromLast);
        while (distance < sum)
        {
            var color = Color.White;
            var mlro = new MeasurementLineRenderObject(distance, distance + interval, color);
            RenderObjects.Add(mlro);
            distance += interval*2;
        }
    }
    
    public override List<RenderObject> GetRenderObjects()
    {
        return RenderObjects.ToList();
    }

    public override void Stop()
    {
    }

    private async Task UpdateRenderObjects(CancellationToken cancellationToken)
    {
        //create list of both button and interval measurement lines in a list of RenderObjects
        RenderObjects.Clear();
        await UpdateSettingsAsync(cancellationToken);
    }

    public override async Task UpdateAsync(TimeSpan timeSpan, CancellationToken cancellationToken)
    {
        //extension from using GlideNGlow.Common.Extensions;
        _timeElapsed += timeSpan.TotalSeconds();
        if (_timeElapsed > _updateInterval)
        {
            _timeElapsed = 0;
            await UpdateRenderObjects(cancellationToken);
        }
    }

    public override Task ButtonPressed(int id, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}