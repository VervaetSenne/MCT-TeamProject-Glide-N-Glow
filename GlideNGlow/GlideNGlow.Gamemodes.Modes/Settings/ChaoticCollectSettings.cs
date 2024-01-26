using GlideNGlow.Common.Extensions;
using Newtonsoft.Json;

namespace GlideNGlow.Gamemodes.Modes.Settings;

public class ChaoticCollectSettings
{
    [JsonConstructor]
    public ChaoticCollectSettings(string playerAmount, string timeLimit)
    {
        PlayerAmount = int.Parse(playerAmount);
        TimeLimit = TimeSpan.Parse(timeLimit).TotalSeconds();
    }
    
    public int PlayerAmount { get; set; }
    public float TimeLimit { get; set; }
}