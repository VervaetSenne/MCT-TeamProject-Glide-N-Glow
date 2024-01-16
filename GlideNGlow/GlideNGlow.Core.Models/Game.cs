namespace GlideNGlow.Core.Models;

public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required byte[] Image { get; set; }
    public required string AssemblyName { get; set; }
    public required string Settings { get; set; }
}