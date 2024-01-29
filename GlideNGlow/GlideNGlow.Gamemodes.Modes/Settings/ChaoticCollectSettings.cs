using GlideNGlow.Common.Enums;
using GlideNGlow.Common.Extensions;
using Newtonsoft.Json;

namespace GlideNGlow.Gamemodes.Modes.Settings;

public class ChaoticCollectSettings : IHasPlayers<int>
{
    [JsonConstructor]
    public ChaoticCollectSettings(string playerAmount, string timeLimit)
    {
        PlayerAmount = int.Parse(playerAmount);
        TimeLimit = TimeSpan.ParseExact(timeLimit, @"%m\:%s", null).TotalSeconds();
    }
    
    public int PlayerAmount { get; set; }
    public float TimeLimit { get; set; }
}