using GlideNGlow.Core.Models;

namespace GlideNGlow.Core.Services.Abstractions;

public interface IGameService
{
    Task<IEnumerable<Game>> FindAsync();
    Task<Game?> FindByIdAsync(Guid? gameId);
    Task<IEnumerable<Game>> FindByIdAsync(IList<Guid> availableGamemodes);
}