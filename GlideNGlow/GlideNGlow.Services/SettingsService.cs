using System.Text.Json;
using GlideNGlow.Common.Enums;
using GlideNGlow.Common.Extensions;
using GlideNGlow.Common.Models;
using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Common.Options.Extensions;
using GlideNGlow.Core.Dto;
using GlideNGlow.Core.Dto.Requests;
using GlideNGlow.Core.Dto.Results;
using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Services.Abstractions;
using Microsoft.Extensions.Options.Implementations;
using Newtonsoft.Json;

namespace GlideNGlow.Services;

public class SettingsService : ISettingsService
{
    private readonly IWritableOptions<AppSettings> _appSettings;
    private readonly IGameService _gameService;

    private AppSettings AppSettings => _appSettings.GetCurrentValue();

    public SettingsService(IWritableOptions<AppSettings> appSettings, IGameService gameService)
    {
        _appSettings = appSettings;
        _gameService = gameService;
    }

    private static int LargestConcurrent(IEnumerable<int> all)
    {
        all = all.ToList();
        return Enumerable.Range(0, all.Max() + 2).Except(all).First();
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

    public void UpdateCurrentGamemode(Guid? gameId, JsonElement? settings)
    {
        _appSettings.Update(s =>
        {
            s.CurrentGamemode = gameId;
            s.CurrentSettings = s.CurrentGamemode is null || settings is null
                ? string.Empty
                : settings.Value.ToString();
        });
    }

    public Guid? GetCurrentGamemode()
    {
        Guid? current = null;
        _appSettings.Update(s => current = s.CurrentGamemode);
        return current;
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
                button.DistanceFromStart = distance ?? 0;
                if (distance is null)
                {
                    button.ButtonNumber = -1;
                }
                else if (button.ButtonNumber == -1)
                {
                    button.ButtonNumber = LargestConcurrent(s.Buttons.Select(b => b.ButtonNumber).Cast<int>());
                }
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
        var lightstrip = new LightstripData
        {
            Id = lightstrips.Count > 1
                ? LargestConcurrent(lightstrips.Select(l => l.Id))
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

    public async Task<ContentDto?> GetContentAsync(Guid gameId)
    {
        var game = await _gameService.FindByIdAsync(gameId);
        
        if (game is null) return null;

        if (game.ContentType is ContentType.None)
        {
            return new ContentDto
            {
                Type = game.ContentType,
            };
        }
        
        var settings = string.Empty;
        _appSettings.Update(s => settings = s.CurrentSettings);

        var players = JsonConvert.DeserializeObject<IHasPlayers>(settings);

        return new ContentDto
        {
            Type = game.ContentType,
            Value = players?.PlayerAmount
        };
    }
}