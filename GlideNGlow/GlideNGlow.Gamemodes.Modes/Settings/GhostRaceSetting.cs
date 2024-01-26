using GlideNGlow.Common.Extensions;
using Newtonsoft.Json;

namespace GlideNGlow.Gamemodes.Modes.Settings;

public class GhostRaceSetting
{
    [JsonConstructor]
    public GhostRaceSetting(string timeLimit)
    {
        TimeLimit = TimeSpan.Parse(timeLimit).TotalSeconds();
    }
    public float TimeLimit { get; set; }
}