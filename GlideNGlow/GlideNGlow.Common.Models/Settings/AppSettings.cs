namespace GlideNGlow.Common.Models.Settings;

public class AppSettings
{
    public string Ip { get; set; }
    public Guid? CurrentGamemode { get; set; }
    public List<Guid> AvailableGamemodes { get; set; } = new();
    public Guid? ForceGamemode { get; set; }
    public bool AllowUserSwitching { get; set; } = true;
    public bool LightingToggle { get; set; } = true;
    public List<LightButtonData> Buttons { get; set; } = new();
    public List<LightstripData> Strips { get; set; } = new();
}