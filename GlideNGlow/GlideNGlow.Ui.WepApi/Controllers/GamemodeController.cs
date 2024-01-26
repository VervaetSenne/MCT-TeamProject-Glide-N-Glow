using GlideNGlow.Core.Dto;
using GlideNGlow.Core.Models.Extensions;
using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Services.Abstractions;
using GlideNGlow.Socket.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("gamemode")]
[ApiController]
public class GamemodeController : Controller
{
    private readonly ISettingsService _settingsService;
    private readonly IAvailableGameService _availableGameService;
    private readonly IGameService _gameService;
    private readonly ISocketWrapper _socketWrapper;

    public GamemodeController(ISettingsService settingsService, IAvailableGameService availableGameService,
        IGameService gameService, ISocketWrapper socketWrapper)
    {
        _settingsService = settingsService;
        _availableGameService = availableGameService;
        _gameService = gameService;
        _socketWrapper = socketWrapper;
    }

    [HttpPut("allow-switching/{value:bool}")]
    public IActionResult AllowUserSwitch([FromRoute] bool value)
    {
        _settingsService.UpdateAllowSwitching(value);
        return Ok();
    }

    [HttpGet("admin-settings")]
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

    [HttpPatch("force/{gameId}")]
    public async Task<IActionResult> SetForceGamemodeAsync([FromRoute] Guid? gameId)
    {
        _settingsService.UpdateForceGamemode(gameId);
        return Ok(await _availableGameService.GetGamemodesAsync());
    }

    [HttpGet("settings/{gameId:guid}")]
    public async Task<IActionResult> GetGamemodeSettings([FromRoute] Guid gameId)
    {
        var game = await _gameService.FindByIdAsync(gameId);
        if (game is null)
            return BadRequest();

        return Ok(game.GetSettingsObject());
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentGamemodeAsync()
    {
        var currentId = _settingsService.GetCurrentGamemode();
        return Ok(await _gameService.FindByIdAsync(currentId));
    }

    [HttpPost("stop")]
    public Task<IActionResult> StopGamemodeAsync()
    {
        return SetCurrentGamemodeAsync(null, null);
    }

    [HttpPost("current/{gameId}")]
    public async Task<IActionResult> SetCurrentGamemodeAsync([FromRoute] Guid? gameId, [FromBody] Dictionary<string, object>? settings)
    {
        if (!_settingsService.GetAllowSwitching() && _settingsService.GetCurrentGamemode().HasValue)
            return NoContent();
        
        _settingsService.UpdateCurrentGamemode(gameId, settings);
        if (gameId.HasValue)
        {
            var game = await _gameService.FindByIdAsync(gameId);
            if (game is not null) return Ok(game.GetSettingsObject());
        }

        await _socketWrapper.PublishUpdateGamemode(gameId);
        return Ok();
    }
}