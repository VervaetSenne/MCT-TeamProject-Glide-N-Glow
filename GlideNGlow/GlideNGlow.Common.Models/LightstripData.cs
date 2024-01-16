namespace GlideNGlow.Common.Models;

public struct LightstripData
{
    public int Id { get; set; }
    public float DistanceFromLast { get; set; }
    public float Length { get; set; }
    public int Leds { get; set; }
}