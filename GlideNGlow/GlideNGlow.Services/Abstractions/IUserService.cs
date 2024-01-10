using GlideNGlow.Core.Models;

namespace GlideNGlow.Services.Abstractions;

public interface IUserService
{
    Task<IEnumerable<Game>> GetGamemodesAsync();
    Task AddScoreAsync(Guid gameId, string name, string score);
    Task<IEnumerable<Entry>> GetEntriesAsync();
    Task<Game?> GetActiveGamemodeAsync();
    Task SetActiveGamemodeAsync(Guid id);
}