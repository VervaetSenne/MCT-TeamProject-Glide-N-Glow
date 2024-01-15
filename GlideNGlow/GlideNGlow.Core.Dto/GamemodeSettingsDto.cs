namespace GlideNGlow.Core.Dto;

public class GamemodeSettingsDto
{
    public required bool AllowUserSwitching { get; set; }
    public required IEnumerable<GamemodeItemDto> Gamemodes { get; set; }
}