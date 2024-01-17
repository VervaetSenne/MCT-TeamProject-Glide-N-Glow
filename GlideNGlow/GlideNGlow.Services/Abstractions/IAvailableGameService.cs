using GlideNGlow.Core.Dto;

namespace GlideNGlow.Services.Abstractions;

public interface IAvailableGameService
{
    Task<IEnumerable<GamemodeItemDto>> GetGamemodesAsync();
    Task<IEnumerable<GamemodeItemDto>> GetAvailableGamemodesAsync();
}