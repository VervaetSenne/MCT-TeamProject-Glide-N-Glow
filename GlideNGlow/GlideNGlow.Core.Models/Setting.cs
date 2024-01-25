using GlideNGlow.Core.Models.Enums;

namespace GlideNGlow.Core.Models;

public class Setting
{
    public required string DisplayName { get; set; }
    public required string Name { get; set; }
    public required SettingType Type { get; set; }
}