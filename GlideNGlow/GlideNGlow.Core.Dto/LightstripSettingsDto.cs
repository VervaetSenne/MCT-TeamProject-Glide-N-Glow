namespace GlideNGlow.Core.Dto;

public class LightstripSettingsDto
{
    public required bool SamePiece { get; set; }
    public required bool OnePiece { get; set; }
    public required IEnumerable<LightstripDto> Lightstrips { get; set; }
}