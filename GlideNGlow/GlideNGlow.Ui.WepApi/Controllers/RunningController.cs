using GlideNGlow.Socket.Data;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("running")]
[ApiController]
public class RunningController : Controller
{
    private readonly ScoreHandler _scoreHandler;

    public RunningController(ScoreHandler scoreHandler)
    {
        _scoreHandler = scoreHandler;
    }

    [HttpGet("scores")]
    public IActionResult GetScores()
    {
        return Ok(_scoreHandler.Scores);
    }
}