namespace GlideNGlow.GPIO.Models;

public struct LightPixel
{
    public bool On { get; set; }
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }

    public LightPixel()
    {
        R = 0;
        G = 0;
        B = 0;
    }

    public LightPixel(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    public LightPixel(string rgb)
    {
        string[] rgbArray = rgb.Split(',');
        R = byte.Parse(rgbArray[0]);
        G = byte.Parse(rgbArray[1]);
        B = byte.Parse(rgbArray[2]);
    }

    public override string ToString()
    {
        return $"R: {R}, G: {G}, B: {B}";
    }
}