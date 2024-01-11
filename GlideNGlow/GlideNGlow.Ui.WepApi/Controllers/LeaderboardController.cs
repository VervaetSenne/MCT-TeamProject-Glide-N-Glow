using GlideNGlow.Core.Models;
using GlideNGlow.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("leaderboard")]
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
        return Ok(leaderBoard);
    }

    [HttpPost("{gameId:guid}/{name}/{score}")]
    public async Task<IActionResult> AddScoreAsync(Guid gameId, string name, string score)
    {
        await _userService.AddScoreAsync(gameId, name, score);
        return Ok();
    }
}