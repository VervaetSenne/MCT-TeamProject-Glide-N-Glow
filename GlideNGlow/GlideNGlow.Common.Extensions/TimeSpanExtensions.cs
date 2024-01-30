namespace GlideNGlow.Common.Extensions;

public static class TimeSpanExtensions
{
    public static float TotalSeconds(this TimeSpan timeSpan)
    {
        return (float) timeSpan.TotalSeconds;
    }
}