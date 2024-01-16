using GlideNGlow.Core.Data;
using GlideNGlow.Core.Models;
using GlideNGlow.Core.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GlideNGlow.Core.Services;

public class GameService : IGameService
{
    private readonly GlideNGlowDbContext _dbContext;

    public GameService(GlideNGlowDbContext dbContext)
    {
        _dbContext = dbContext;
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
}