using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Services.Abstractions;
using GlideNGlow.Ui.WepApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("leaderboard")]
[ApiController]
public class LeaderboardController : Controller
{
    private readonly IEntryService _entryService;
    private readonly IAvailableGameService _availableGameService;

    public LeaderboardController(IEntryService entryService, IAvailableGameService availableGameService)
    {
        _entryService = entryService;
        _availableGameService = availableGameService;
    }

    [HttpGet("gamemodes")]
    public async Task<IActionResult> GetGamemodesAsync()
    {
        var gamemodes = await _availableGameService.GetLeaderboardAsync();
        return Ok(gamemodes);
    }

    [HttpGet("{mode}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid mode, [FromQuery] EntryFilter filter)
    {
        var entries = await _entryService.FindFromGameAsync(mode, filter.TimeFrame, filter.Unique, filter.Username);
        return Ok(entries);
    }
}