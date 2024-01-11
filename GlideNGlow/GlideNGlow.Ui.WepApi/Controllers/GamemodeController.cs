using GlideNGlow.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("gamemode")]
[ApiController]
public class GamemodeController : Controller
{
    private readonly IUserService _userService;

    public GamemodeController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAync()
    {
        var games = await _userService.GetGamemodesAsync();
        return Ok(games);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var current = await _userService.GetActiveGamemodeAsync();
        return Ok(current);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableAsync()
    {
        var result = await _userService.GetGamemodesAsync();
        return Ok(result);
    }
}