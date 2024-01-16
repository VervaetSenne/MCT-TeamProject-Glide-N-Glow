namespace GlideNGlow.Common.Models;

public struct LightStripData
{
    public List<float> DistanceFromLast { get; set; }
    public List<float> Length { get; set; }
    public List<int> Leds { get; set; }
}