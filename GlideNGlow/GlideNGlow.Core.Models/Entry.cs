namespace GlideNGlow.Core.Models;

public class Entry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GameId { get; set; }
    public DateTime DateTime { get; set; }
    public required string Name { get; set; }
    public required string Score { get; set; }

    public Game Game { get; set; } = null!;
}