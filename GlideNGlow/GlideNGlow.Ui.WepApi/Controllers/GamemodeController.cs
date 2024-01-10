using GlideNGlow.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("[controller]")]
[ApiController]
public class GamemodeController : Controller
{
    private readonly IUserService _userService;

    public GamemodeController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAync()
    {
        var games = await _userService.GetGamemodesAsync();
        return new OkObjectResult(games);
    }
}