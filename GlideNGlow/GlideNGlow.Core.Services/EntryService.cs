﻿using GlideNGlow.Core.Data;
using GlideNGlow.Core.Dto;
using GlideNGlow.Core.Models;
using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Core.Services.Enums;
using GlideNGlow.Core.Services.Generic;
using Microsoft.EntityFrameworkCore;
using MoreAsyncLINQ;

namespace GlideNGlow.Core.Services;

public class EntryService : IEntryService
{
    private readonly GlideNGlowDbContext _dbContext;

    public EntryService(GlideNGlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    private static bool IsWithinTimeFrame(Entry entry, TimeFrame timeFrame)
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

    private static string FormatScore(string score)
    {
        return score.Contains(':')
            ? !TimeSpan.TryParseExact(score, @"%h\:%m\:%s\.fff", null, out var timeSpan)
                ? !TimeSpan.TryParseExact(score, @"%m\:%s\.fff", null, out timeSpan)
                    ? !TimeSpan.TryParseExact(score, @"%s\.fff", null, out timeSpan)
                        ? score
                        : ToString(timeSpan)
                    : ToString(timeSpan)
                : ToString(timeSpan)
            : float.Parse(score).ToString("F");

        string ToString(TimeSpan t)
        {
            return t.ToString(@"mm\:ss\.fff");
        }
    }

    public async Task<IEnumerable<EntryDto>> FindFromGameAsync(Guid mode, TimeFrame timeFrame, bool unique,
        string username)
    {
        var entries = _dbContext.Entries
            .Where(e => e.GameId == mode)
            .Include(e => e.Game)
            .AsAsyncEnumerable();

        if (unique)
            entries = entries.DistinctBy(e => e.Name);

        if (timeFrame != TimeFrame.All)
            entries = entries.Where(tuple => IsWithinTimeFrame(tuple, timeFrame));

        return await entries
            .Select(e =>
            {
                e.Score = FormatScore(e.Score);
                return e;
            })
            .OrderBy(e => e, new EntryComparer())
            .Select((e, i) => new EntryDto
            {
                Rank = i + 1,
                Username = e.Name,
                Score = e.Score
            })
            .Where(e => e.Username.ToLower().Contains(username.Trim()))
            .ToListAsync();
    }

    public async Task<IEnumerable<Entry>> GetBestScoresAsync(IList<Guid> availableGamemodes)
    {
        if (availableGamemodes.Count == 0)
            return Enumerable.Empty<Entry>();

        var entries = await _dbContext.Entries
            .Where(e => availableGamemodes.Any(id => e.GameId == id))
            .Include(e => e.Game)
            .ToListAsync();

        return entries
            .Select(e =>
            {
                e.Score = FormatScore(e.Score);
                return e;
            })
            .GroupBy(e => e.GameId)
            .Select(g => g.Min(new EntryComparer()) ?? new Entry
            {
                GameId = g.Key,
                DateTime = DateTime.MinValue,
                Name = string.Empty,
                Score = "----"
            });
    }

    public async Task<Entry> AddEntryAsync(Guid gameId, string playerName, string score)
    {
        var entry = new Entry
        {
            GameId = gameId,
            DateTime = DateTime.Now,
            Name = playerName,
            Score = score
        };
        _dbContext.Entries.Add(entry);
        await _dbContext.SaveChangesAsync();
        
        return entry;
    }
}