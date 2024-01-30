namespace GlideNGlow.Common.Models;

public class LightButtonData
{
    public int? ButtonNumber { get; set; }
    public required string MacAddress { get; init; }
    public float? DistanceFromStart { get; set; }
}