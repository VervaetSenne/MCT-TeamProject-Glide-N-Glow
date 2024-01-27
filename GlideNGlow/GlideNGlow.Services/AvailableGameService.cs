using GlideNGlow.Common.Enums;
using GlideNGlow.Core.Dto;
using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Services.Abstractions;

namespace GlideNGlow.Services;

public class AvailableGameService : IAvailableGameService
{
    private readonly ISettingsService _settingsService;
    private readonly IGameService _gameService;
    private readonly IEntryService _entryService;

    public AvailableGameService(ISettingsService settingsService, IGameService gameService, IEntryService entryService)
    {
        _settingsService = settingsService;
        _gameService = gameService;
        _entryService = entryService;
    }

    public async Task<IEnumerable<GamemodeItemDto>> GetAsync()
    {
        var availableGamemodes = _settingsService.GetAvailableGamemodes();
        var forcedGamemode = _settingsService.GetForcedGamemode();
        var gamemodes = await _gameService.FindAsync();

        return gamemodes.Select(g => new GamemodeItemDto
        {
            Force = forcedGamemode is not null && forcedGamemode == g.Id,
            Available = forcedGamemode is null && availableGamemodes.Any(ag => ag == g.Id),
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            Settings = g.Settings
        });
    }

    public async Task<IEnumerable<GamemodeItemDto>> GetAvailableAsync()
    {
        var availableGamemodes = _settingsService.GetAvailableGamemodes().ToList();
        var forcedGamemode = _settingsService.GetForcedGamemode();
        var gamemodes = await _gameService.FindByIdAsync(availableGamemodes);
        var bestScores = await _entryService.GetBestScoresAsync(availableGamemodes);
        
        return gamemodes.Select(g => new GamemodeItemDto
        {
            Force = forcedGamemode is not null && forcedGamemode == g.Id,
            Available = true,
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            Settings = g.Settings,
            BestScore = bestScores.FirstOrDefault(e => e.GameId == g.Id)?.Score ?? "----"
        });
    }

    public async Task<IEnumerable<GamemodeItemDto>> GetLeaderboardAsync()
    {
        var availableGamemodes = _settingsService.GetAvailableGamemodes().ToList();
        var forcedGamemode = _settingsService.GetForcedGamemode();
        var gamemodes = (await _gameService.FindByIdAsync(availableGamemodes)).Where(g => g.ScoreImportance != ScoreImportance.None).ToList();
        var bestScores = await _entryService.GetBestScoresAsync(gamemodes.Select(g => g.Id).ToList());
        
        return gamemodes.Select(g => new GamemodeItemDto
        {
            Force = forcedGamemode is not null && forcedGamemode == g.Id,
            Available = true,
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            Settings = g.Settings,
            BestScore = bestScores.FirstOrDefault(e => e.GameId == g.Id)?.Score ?? "----"
        });
    }
}