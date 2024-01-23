﻿using GlideNGlow.Common.Extensions;
using GlideNGlow.Common.Models;
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

    private List<LightstripDto> GetLightstrips()
    {
        return AppSettings.Strips
            .Select(l => new LightstripDto
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

    public LightstripDto AddLightStrip(bool samePiece, bool onePiece)
    {
        var lightstrips = GetLightstrips();
        var lightstrip = new LightstripData
        {
            Id = lightstrips.Count > 1
                ? Enumerable.Range(0, lightstrips.MaxBy(l => l.Id)!.Id + 1).Except(lightstrips.Select(l => l.Id)).First()
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
        
        return new LightstripDto
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

    public bool UpdateLightStrip(int lightId, LightstripDto lightstrip)
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
                Id = lightstrip.Id,
                DistanceFromLast = lightstrip.Distance,
                Length = lightstrip.Length,
                Leds = lightstrip.Pixels
            };
        });
        return isUpdated;
    }
}