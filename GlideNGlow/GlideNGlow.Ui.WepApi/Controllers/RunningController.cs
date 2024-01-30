using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Services.Abstractions;
using GlideNGlow.Socket;
using GlideNGlow.Socket.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("running")]
[ApiController]
public class RunningController : Controller
{
    private readonly ScoreHandler _scoreHandler;
    private readonly ISettingsService _settingsService;
    private readonly ISocketWrapper _socketWrapper;
    private readonly IEntryService _entryService;

    public RunningController(ScoreHandler scoreHandler, ISettingsService settingsService, ISocketWrapper socketWrapper, IEntryService entryService)
    {
        _scoreHandler = scoreHandler;
        _settingsService = settingsService;
        _socketWrapper = socketWrapper;
        _entryService = entryService;
    }

    [HttpGet("content/{gameId:guid}")]
    public async Task<IActionResult> GetContent([FromRoute] Guid gameId)
    {
        var content = await _settingsService.GetContentAsync(gameId);
        return Ok(content);
    }

    [HttpPost("score/{id:int}")]
    public async Task<IActionResult> ClaimScore(int id, [FromQuery] string playerName)
    {
        var score = _scoreHandler.ClaimScore(id, playerName);
        if (score is null)
            return BadRequest();
        
        await _entryService.AddEntryAsync(_scoreHandler.GameId, score.PlayerName, score.Value);
        await _socketWrapper.PublishScoreClaimedAsync(score.PlayerIndex, score.PlayerName);
        return Ok();
    }

    [HttpGet("scores")]
    public IActionResult GetScores()
    {
        return Ok(_scoreHandler.Scores);
    }
}