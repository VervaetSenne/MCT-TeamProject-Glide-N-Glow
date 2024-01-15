namespace GlideNGlow.Common.Models.Settings;

public class AppSettings
{
    public string Ip { get; set; }
    public Guid? CurrentGamemode { get; set; }
    public List<Guid> AvailableGamemodes { get; set; } = new();
    public Guid? ForceGamemode { get; set; }
    public bool AllowUserSwitching { get; set; }
    public bool LightingToggle { get; set; } = true;
    public List<LightButtonData> Strips { get; set; } = new();
    public List<LightStripData> Buttons { get; set; } = new();
}