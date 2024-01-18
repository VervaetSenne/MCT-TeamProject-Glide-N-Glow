using GlideNGlow.Core.Dto;
using GlideNGlow.Core.Services.Enums;

namespace GlideNGlow.Core.Services.Abstractions;

public interface IEntryService
{
    Task<IEnumerable<EntryDto>> FindFromGameAsync(string? mode, TimeFrame timeFrame, bool unique, string username);
}