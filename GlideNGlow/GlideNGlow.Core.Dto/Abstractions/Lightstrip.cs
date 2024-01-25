namespace GlideNGlow.Core.Dto.Abstractions;

public abstract class Lightstrip
{
    public required float Distance { get; set; }
    public required float Length { get; set; }
    public required int Pixels { get; set; }
}