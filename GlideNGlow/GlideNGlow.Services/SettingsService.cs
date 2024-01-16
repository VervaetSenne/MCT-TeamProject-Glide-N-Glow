using GlideNGlow.Common.Extensions;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Core.Dto;
using GlideNGlow.Services.Abstractions;
using Microsoft.Extensions.Options.Implementations;

namespace GlideNGlow.Services;

public class SettingsService : ISettingsService
{
    private readonly IWritableOptions<AppSettings> _appSettings;

    private AppSettings AppSettings => _appSettings.CurrentValue;

    public SettingsService(IWritableOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public bool UpdateAllowSwitching(bool value)
    {
        _appSettings.Update(s => s.AllowUserSwitching = value);
        return value;
    }

    public IEnumerable<Guid> GetAvailableGamemodes()
    {
        return AppSettings.AvailableGamemodes;
    }

    public Guid? GetForcedGamemode()
    {
        return AppSettings.ForceGamemode;
    }

    public bool GetAllowSwitching()
    {
        return AppSettings.AllowUserSwitching;
    }

    public bool TryAddAvailable(Guid gameId)
    {
        if (AppSettings.AvailableGamemodes.Contains(gameId))
            return false;
        
        _appSettings.Update(s => s.AvailableGamemodes.Add(gameId));
        return true;
    }

    public bool TryRemoveAvailable(Guid gameId)
    {
        var isRemoved = false;
        _appSettings.Update(s => isRemoved = s.AvailableGamemodes.Remove(gameId));
        return isRemoved;
    }

    public void UpdateForceGamemode(Guid? gameId)
    {
        _appSettings.Update(s =>
        {
            s.ForceGamemode = gameId;
            if (gameId is not null)
                s.AvailableGamemodes = new List<Guid>
                {
                    gameId.Value
                };
        });
    }

    public void UpdateCurrentGamemode(Guid? gameId)
    {
        _appSettings.Update(s => s.CurrentGamemode = gameId);
    }

    public IEnumerable<ButtonDto> GetButtons()
    {
        return AppSettings.Buttons
            .OrderBy(l => l.ButtonNumber)
            .Select(l => new ButtonDto
            {
                Id = l.MacAddress.MacToHex(),
                Distance = l.DistanceFromStart
            });
    }

    public void UpdateButton(string buttonId, float? distance)
    {
        _appSettings.Update(s =>
        {
            var button = s.Buttons.FirstOrDefault(l => l.MacAddress.MacToHex() == buttonId);
            if (button is not null)
            {
                button.DistanceFromStart = distance;
            }
        });
    }
}