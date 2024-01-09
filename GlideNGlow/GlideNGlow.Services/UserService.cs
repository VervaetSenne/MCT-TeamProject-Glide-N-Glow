using GlideNGlow.Core.Models;
using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Services.Settings;
using Microsoft.Extensions.Options.Implementations;

namespace GlideNGlow.Services;

public class UserService
{
    private readonly IWritableOptions<AppSettings> _appsettings;
    private readonly IEntryService _entryService;
    private readonly IGameService _gameService;

    private AppSettings AppSettings => _appsettings.CurrentValue;

    public UserService(IWritableOptions<AppSettings> appsettings, IEntryService entryService, IGameService gameService)
    {
        _appsettings = appsettings;
        _entryService = entryService;
        _gameService = gameService;
    }

    public async Task AddScoreAsync(Guid gameId, string name, string score)
    {
        var game = await _gameService.FindByIdAsync(gameId);
        if (game is null)
            throw new Exception("Game could not be found!"); // TODO create a service result to return.
        await _entryService.CreateAsync(new Entry
        {
            GameId = gameId,
            Name = name,
            Score = score
        });
    }
    
    public async Task<Game?> GetActiveGamemodeAsync()
    {
        if (AppSettings.CurrentGamemode is null)
            return null;
        return await _gameService.FindByIdAsync(AppSettings.CurrentGamemode.Value);
    }

    public async Task SetActiveGamemodeAsync(Guid id)
    {
        _appsettings.Update(settings => settings.CurrentGamemode = id);
        // TODO continue with massimo code
    }

    public async Task SetAvailableGamemodesAsync(IEnumerable<Guid> ids)
    {
        _appsettings.Update(settings => settings.AvailableGamemodes = ids.ToList());
        // TODO continue with massimo code
    }
}