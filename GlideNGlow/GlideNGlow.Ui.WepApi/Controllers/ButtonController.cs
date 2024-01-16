using GlideNGlow.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("button")]
[ApiController]
public class ButtonController : Controller
{
    private readonly ISettingsService _settingsService;

    public ButtonController(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet]
    public IActionResult GetButtons()
    {
        var buttons = _settingsService.GetButtons();
        return Ok(buttons);
    }

    [HttpPut("{buttonId}")]
    public IActionResult UpdateButton([FromRoute] string buttonId, [FromQuery] float? distance)
    {
        _settingsService.UpdateButton(buttonId, distance);
        return Ok();
    }

    [HttpDelete("{buttonId}")]
    public IActionResult DeleteButton(string buttonId)
    {
        return UpdateButton(buttonId, null);
    }
}