using GlideNGlow.Common.Models.Settings;
using Microsoft.Extensions.Options;

namespace GlideNGlow.Common.Options.Extensions;

public static class OptionsExtensions
{
    public static AppSettings GetCurrentValue(this AppSettings settings)
    {
        settings.Buttons = settings.Buttons
            .DistinctBy(data => data.MacAddress)
            .ToList();
        settings.Strips = settings.Strips
            .DistinctBy(data => data.Id)
            .ToList();
        settings.AvailableGamemodes = settings.AvailableGamemodes
            .Distinct()
            .ToList();
        return settings;
    }
    
    public static AppSettings GetCurrentValue(this IOptionsMonitor<AppSettings> options)
    {
        return options.CurrentValue.GetCurrentValue();
    }
}