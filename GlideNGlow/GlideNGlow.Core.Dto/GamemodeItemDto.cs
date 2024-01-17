namespace GlideNGlow.Core.Dto;

public class GamemodeItemDto
{
    public required bool Available { get; set; }
    public required bool Force { get; set; }
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Settings { get; set; }
}