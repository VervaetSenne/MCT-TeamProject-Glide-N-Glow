using GlideNGlow.Core.Services.Enums;

namespace GlideNGlow.Ui.WepApi.Models;

public class EntryFilter
{
    public TimeFrame TimeFrame { get; set; } = Core.Services.Enums.TimeFrame.All;
    public bool Unique { get; set; } = false;
    public string Username { get; set; } = string.Empty;
}