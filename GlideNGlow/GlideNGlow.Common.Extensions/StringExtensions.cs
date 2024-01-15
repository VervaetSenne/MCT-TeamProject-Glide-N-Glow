namespace GlideNGlow.Common.Extensions;

public static class StringExtensions
{
    private static readonly Random Random = new();
    
    public static string MacToHex(this string mac)
    {
        var hexs = mac.Replace(":", "").StringToByteArray().ToList();
        var id = string.Empty;
        for (var i = 0; i < hexs.Count / 2; i++)
        {
            id += ((hexs[i] + hexs[i + 1] + Random.Next(255)) % 255).ToString("X2");
        }

        return id;
    }
    
    public static byte[] StringToByteArray(this string hex)
    {
        if (hex.Length % 2 == 1)
            throw new Exception("The binary key cannot have an odd number of digits");

        var arr = new byte[hex.Length >> 1];

        for (var i = 0; i < hex.Length >> 1; ++i)
        {
            arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + GetHexVal(hex[(i << 1) + 1]));
        }

        return arr;
    }

    public static int GetHexVal(this char hex)
    {
        int val = hex;
        return char.IsLower(hex)
            ? val - (val < 58 ? 48 : 87)
            : val - (val < 58 ? 48 : 55);
    }
}