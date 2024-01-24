using GlideNGlow.Core.Models.Enums;

namespace GlideNGlow.Core.Models;

public class Entry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid GameId { get; set; }
    public required DateTime DateTime { get; set; } = DateTime.Now;
    public required string Name { get; set; }
    public required string Score { get; set; }

public Game Game { get; set; } = null!;
}