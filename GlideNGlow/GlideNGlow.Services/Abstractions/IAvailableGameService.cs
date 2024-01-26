using GlideNGlow.Core.Dto;

namespace GlideNGlow.Services.Abstractions;

public interface IAvailableGameService
{
    Task<IEnumerable<GamemodeItemDto>> GetAsync();
    Task<IEnumerable<GamemodeItemDto>> GetAvailableAsync();
    Task<IEnumerable<GamemodeItemDto>> GetLeaderboardAsync();
}