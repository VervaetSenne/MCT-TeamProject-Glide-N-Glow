using GlideNGlow.Core.Dto;
using GlideNGlow.Core.Models;
using GlideNGlow.Core.Services.Enums;

namespace GlideNGlow.Core.Services.Abstractions;

public interface IEntryService
{
    Task<IEnumerable<EntryDto>> FindFromGameAsync(Guid mode, TimeFrame timeFrame, bool unique, string username);
    IAsyncEnumerable<Entry> GetBestScores(IList<Guid> availableGamemodes);
}