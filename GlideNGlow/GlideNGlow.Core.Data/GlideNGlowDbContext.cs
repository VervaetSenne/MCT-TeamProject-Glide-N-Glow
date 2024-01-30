using GlideNGlow.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GlideNGlow.Core.Data;

public sealed class GlideNGlowDbContext : DbContext
{
    public DbSet<Entry> Entries { get; }
    public DbSet<Game> Games { get; }

    public GlideNGlowDbContext(DbContextOptions options) : base(options)
    {
        Entries = Set<Entry>();
        Games = Set<Game>();
    }
}