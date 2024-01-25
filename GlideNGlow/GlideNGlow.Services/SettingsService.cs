using GlideNGlow.Common.Extensions;
using GlideNGlow.Common.Models;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Common.Options.Extensions;
using GlideNGlow.Core.Dto;
using GlideNGlow.Core.Dto.Abstractions;
using GlideNGlow.Core.Dto.Requests;
using GlideNGlow.Core.Dto.Results;
using GlideNGlow.Services.Abstractions;
using Microsoft.Extensions.Options.Implementations;

namespace GlideNGlow.Services;

public class SettingsService : ISettingsService
{
    private readonly IWritableOptions<AppSettings> _appSettings;

    private AppSettings AppSettings => _appSettings.GetCurrentValue();

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

    public Guid? GetCurrentGamemode()
    {
        return AppSettings.CurrentGamemode;
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

    private List<LightstripResultDto> GetLightstrips()
    {
        return AppSettings.Strips
            .Select(l => new LightstripResultDto
            {
                Id = l.Id,
                Distance = l.DistanceFromLast,
                Length = l.Length,
                Pixels = l.Leds
            })
            .ToList();
    }

    public LightstripSettingsDto GetLightstripSettings()
    {
        var lightstrips = GetLightstrips();

        return new LightstripSettingsDto
        {
            SamePiece = lightstrips.DistinctBy(l => (l.Length, l.Pixels)).Count() != lightstrips.Count,
            OnePiece = lightstrips.All(l => l.Distance == 0),
            Lightstrips = lightstrips
        };
    }

    public LightstripResultDto AddLightStrip(bool samePiece, bool onePiece)
    {
        var lightstrips = GetLightstrips();
        var largestId = lightstrips.MaxBy(l => l.Id)!.Id + 1;
        var lightstrip = new LightstripData
        {
            Id = lightstrips.Count > 1
                ? Enumerable.Range(0, largestId + 1).Except(lightstrips.Select(l => l.Id)).First()
                : 0
        };

        if (samePiece || onePiece)
        {
            var last = lightstrips.Last();
            if (samePiece)
            {
                lightstrip.Leds = last.Pixels;
                lightstrip.Length = last.Length;
            }

            if (onePiece)
            {
                lightstrip.DistanceFromLast = 0;
            }
        }
        
        _appSettings.Update(s =>
        {
            s.Strips.Add(lightstrip);
        });
        
        return new LightstripResultDto
        {
            Id = lightstrip.Id,
            Distance = lightstrip.DistanceFromLast,
            Length = lightstrip.Length,
            Pixels = lightstrip.Leds
        };
    }

    public bool TryRemoveLightstrip(int lightId)
    {
        var isRemoved = false;
        _appSettings.Update(s => isRemoved = s.Strips.RemoveAll(l => l.Id == lightId) > 0);
        return isRemoved;
    }

    public bool UpdateLightStrip(int lightId, LightstripRequestsDto lightstrip)
    {
        var isUpdated = true;
        _appSettings.Update(s =>
        {
            var strip = s.Strips.FirstOrDefault(l => l.Id == lightId);
            if (strip is null)
            {
                isUpdated = false;
                return;
            }

            var index = s.Strips.IndexOf(strip);
            s.Strips[index] = new LightstripData
            {
                Id = lightId,
                DistanceFromLast = lightstrip.Distance,
                Length = lightstrip.Length,
                Leds = lightstrip.Pixels
            };
        });
        return isUpdated;
    }
}