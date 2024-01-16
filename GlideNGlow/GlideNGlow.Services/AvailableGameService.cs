using GlideNGlow.Core.Dto;
using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Services.Abstractions;

namespace GlideNGlow.Services;

public class AvailableGameService : IAvailableGameService
{
    private readonly ISettingsService _settingsService;
    private readonly IGameService _gameService;

    public AvailableGameService(ISettingsService settingsService, IGameService gameService)
    {
        _settingsService = settingsService;
        _gameService = gameService;
    }

    public async Task<IEnumerable<GamemodeItemDto>> GetAvailableGamemodesAsync()
    {
        var availableGamemodes = _settingsService.GetAvailableGamemodes();
        var forcedGamemode = _settingsService.GetForcedGamemode();
        var gamemodes = await _gameService.FindAsync();

        return gamemodes.Select(g => new GamemodeItemDto
        {
            Force = forcedGamemode is not null && forcedGamemode == g.Id,
            Available = forcedGamemode is not null && availableGamemodes.Any(ag => ag == g.Id)
        });
    }
}