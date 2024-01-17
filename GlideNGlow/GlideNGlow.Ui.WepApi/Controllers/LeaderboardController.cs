using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Ui.WepApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("leaderboard")]
[ApiController]
public class LeaderboardController : Controller
{
    private readonly IEntryService _entryService;

    public LeaderboardController(IEntryService entryService)
    {
        _entryService = entryService;
    }

    [HttpGet("{mode}")]
    public async Task<IActionResult> GetAsync([FromRoute] string? mode, [FromQuery] EntryFilter filter)
    {
        var entries = await _entryService.FindFromGameAsync(mode, filter.TimeFrame, filter.Unique, filter.Username);
        return Ok(entries);
    }
}