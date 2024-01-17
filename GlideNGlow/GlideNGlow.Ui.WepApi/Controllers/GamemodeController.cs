using GlideNGlow.Core.Dto;
using GlideNGlow.Core.Models;
using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("gamemode")]
[ApiController]
public class GamemodeController : Controller
{
    private readonly ISettingsService _settingsService;
    private readonly IAvailableGameService _availableGameService;
    private readonly IGameService _gameService;
    
    public GamemodeController(ISettingsService settingsService, IAvailableGameService availableGameService, IGameService gameService)
    {
        _settingsService = settingsService;
        _availableGameService = availableGameService;
        _gameService = gameService;
    }

    [HttpPut("allow-switching/{value:bool}")]
    public IActionResult AllowUserSwitch([FromRoute] bool value)
    {
        _settingsService.UpdateAllowSwitching(value);
        return Ok();
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettingsAsync()
    {
        return Ok(new GamemodeSettingsDto
        {
            AllowUserSwitching = _settingsService.GetAllowSwitching(),
            Gamemodes = await _availableGameService.GetGamemodesAsync()
        });
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableAsync()
    {
        var gamemodes = await _availableGameService.GetAvailableGamemodesAsync();
        return Ok(gamemodes);
    }

    [HttpPost("available/{gameId:guid}")]
    public IActionResult AddAvailable([FromRoute] Guid gameId)
    {
        if (_settingsService.TryAddAvailable(gameId))
            return Ok();

        return NoContent();
    }

    [HttpDelete("available/{gameId:guid}")]
    public IActionResult RemoveAvailable([FromRoute] Guid gameId)
    {
        if (_settingsService.TryRemoveAvailable(gameId))
            return Ok();

        return NoContent();
    }

    [HttpPatch("force/{gameId:guid?}")]
    public async Task<IActionResult> SetForceGamemodeAsync([FromRoute] Guid? gameId)
    {
        _settingsService.UpdateForceGamemode(gameId);
        return Ok(await _availableGameService.GetGamemodesAsync());
    }

    [HttpPost("current/{gameId:guid?}")]
    public async Task<IActionResult> SetCurrentGamemodeAsync([FromRoute] Guid? gameId)
    {
        _settingsService.UpdateCurrentGamemode(gameId);
        if (gameId.HasValue)
        {
            var game = await _gameService.FindByIdAsync(gameId);
            if (game is not null) return Ok(JsonConvert.DeserializeObject<IEnumerable<Setting>>(game.Settings));
        }
        return Ok();
    }
}