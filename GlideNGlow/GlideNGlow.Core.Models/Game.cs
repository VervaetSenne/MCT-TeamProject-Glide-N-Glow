namespace GlideNGlow.Core.Models;

public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
}