using GlideNGlow.Core.Data;
using GlideNGlow.Core.Dto;
using GlideNGlow.Core.Models;
using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Core.Services.Enums;
using Microsoft.EntityFrameworkCore;

namespace GlideNGlow.Core.Services;

public class EntryService : IEntryService
{
    private readonly GlideNGlowDbContext _dbContext;

    public EntryService(GlideNGlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    private bool IsWithinTimeFrame(Entry entry, TimeFrame timeFrame)
    {
        var timeSpan = DateTime.Now - entry.DateTime;
        return timeFrame switch
        {
            TimeFrame.Hour => timeSpan.TotalHours <= 1,
            TimeFrame.Day => timeSpan.TotalDays <= 1,
            TimeFrame.Week => timeSpan.TotalDays <= 7,
            TimeFrame.Month => timeSpan.TotalDays <=
                               DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month),
            TimeFrame.Year => timeSpan.TotalDays <= 365,
            TimeFrame.All => true,
            _ => throw new ArgumentOutOfRangeException(nameof(timeFrame), timeFrame, null)
        };
    }

    public async Task<IEnumerable<EntryDto>> FindFromGameAsync(string? mode, TimeFrame timeFrame, bool unique,
        string username)
    {
        mode ??= (await _dbContext.Games.FirstAsync()).Name;

        var entries = (await _dbContext.Entries
                .Where(e => string.Equals(e.Game.Name.Trim(), mode.Trim(), StringComparison.InvariantCultureIgnoreCase))
                .ToListAsync())
            .Select((e, i) => (Rank: i + 1, Entry: e))
            .AsEnumerable();

        if (unique)
            entries = entries.DistinctBy(e => e.Entry.Name);

        if (!string.IsNullOrWhiteSpace(username))
            entries = entries.Where(e => e.Entry.Name.ToLower().Contains(username.Trim()));

        if (timeFrame != TimeFrame.All)
            entries = entries.Where(tuple => IsWithinTimeFrame(tuple.Entry, timeFrame));

        return entries
            .Select(e => new EntryDto
            {
                Rank = e.Rank,
                Username = e.Entry.Name,
                Score = e.Entry.Score
            }).ToList();
    }
}