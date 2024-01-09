namespace GlideNGlow.Common.Models.Settings;

public class AppSettings
{
    public string Ip { get; set; }
    public Guid? CurrentGamemode { get; set; }
    public List<Guid> AvailableGamemodes { get; set; } = new();
    public bool ForceGamemode { get; set; } = false;
    public bool LightingToggle { get; set; } = true;
    public List<LightButtonData> Strips { get; set; } = new();
    public List<LightStripData> Buttons { get; set; } = new();
}