using GlideNGlow.Core.Models;
using GlideNGlow.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("[controller]")]
[ApiController]
public class LeaderboardController : Controller
{
    private readonly IUserService _userService;

    public LeaderboardController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var leaderBoard = _userService.GetEntriesAsync();
        return new OkObjectResult(leaderBoard);
    }

    [HttpPost("{gameId:guid}/{name}/{score}")]
    public async Task<IActionResult> AddScoreAsync(Guid gameId, string name, string score)
    {
        var entry = _userService.AddScoreAsync(gameId, name, score);
        return new OkObjectResult(entry);
    }
}