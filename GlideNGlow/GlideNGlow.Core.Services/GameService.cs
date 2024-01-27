using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Core.Data;
using GlideNGlow.Core.Models;
using GlideNGlow.Core.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GlideNGlow.Core.Services;

public class GameService : IGameService
{
    private readonly GlideNGlowDbContext _dbContext;
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public GameService(GlideNGlowDbContext dbContext, IOptionsMonitor<AppSettings> appSettings)
    {
        _dbContext = dbContext;
        _appSettings = appSettings;
    }

    public async Task<IEnumerable<Game>> FindAsync()
    {
        return await _dbContext.Games.ToListAsync();
    }

    public async Task<Game?> FindByIdAsync(Guid? gameId)
    {
        if (!gameId.HasValue)
            return null;
        return await _dbContext.Games.SingleOrDefaultAsync(g => g.Id == gameId);
    }

    public async Task<IEnumerable<Game>> FindByIdAsync(IList<Guid> availableGamemodes)
    {
        if (availableGamemodes.Count == 0)
            return Enumerable.Empty<Game>();

        return await _dbContext.Games.Where(g => availableGamemodes.Any(id => id == g.Id)).ToListAsync();
    }
}