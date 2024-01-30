using GlideNGlow.Core.Dto.Requests;
using GlideNGlow.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("lightstrip")]
[ApiController]
public class LightstripController : Controller
{
    private readonly ISettingsService _settingsService;

    public LightstripController(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet("settings")]
    public IActionResult GetSettingsAsync()
    {
        return Ok(_settingsService.GetLightstripSettings());
    }

    [HttpPost]
    public IActionResult AddLightstrip([FromQuery] bool samePiece, [FromQuery] bool onePiece)
    {
        return Ok(_settingsService.AddLightStrip(samePiece, onePiece));
    }

    [HttpPut("{lightId:int}")]
    public IActionResult UpdateLightStrip([FromRoute] int lightId, [FromBody] LightstripRequestsDto updatedLightstrip)
    {
        if (_settingsService.UpdateLightStrip(lightId, updatedLightstrip))
            return Ok();

        return NoContent();
    }

    [HttpDelete("{lightId:int}")]
    public IActionResult RemoveLightstrip([FromRoute] int lightId)
    {
        if (_settingsService.TryRemoveLightstrip(lightId))
            return Ok();

        return NoContent();
    }
}