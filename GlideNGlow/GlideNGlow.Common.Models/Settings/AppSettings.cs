﻿namespace GlideNGlow.Common.Models.Settings;

public class AppSettings
{
    public string Ip { get; set; } = string.Empty;
    public string FrontendUrl { get; set; } = string.Empty;
    public Guid? CurrentGamemode { get; set; } = null;
    public List<Guid> AvailableGamemodes { get; set; } = new();
    public Guid? ForceGamemode { get; set; } = null;
    public bool AllowUserSwitching { get; set; } = true;
    public bool LightingToggle { get; set; } = true;
    public List<LightButtonData> Buttons { get; set; } = new();
    public List<LightstripData> Strips { get; set; } = new();
    public string CurrentSettings { get; set; } = string.Empty;
}