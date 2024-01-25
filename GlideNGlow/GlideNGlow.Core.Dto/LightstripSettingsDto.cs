using GlideNGlow.Core.Dto.Abstractions;

namespace GlideNGlow.Core.Dto;

public class LightstripSettingsDto
{
    public required bool SamePiece { get; set; }
    public required bool OnePiece { get; set; }
    public required IEnumerable<Lightstrip> Lightstrips { get; set; }
}