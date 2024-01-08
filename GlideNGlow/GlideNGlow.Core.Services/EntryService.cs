using GlideNGlow.Core.Data;
using GlideNGlow.Core.Models;
using GlideNGlow.Core.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GlideNGlow.Core.Services;

public class EntryService : IEntryService
{
    private readonly GlideNGlowDbContext _dbContext;

    public EntryService(GlideNGlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Entry>> FindAsync()
    {
        return await _dbContext.Entries.ToListAsync();
    }

    public async Task<Entry> CreateAsync(Entry entry)
    {
        _dbContext.Entries.Add(entry);
        await _dbContext.SaveChangesAsync();
        return entry;
    }
}