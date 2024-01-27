using System.Text.Json;
using GlideNGlow.Core.Dto;
using GlideNGlow.Core.Dto.Requests;
using GlideNGlow.Core.Dto.Results;

namespace GlideNGlow.Services.Abstractions;

public interface ISettingsService
{
    bool GetAllowSwitching();
    bool UpdateAllowSwitching(bool value);
    
    Guid? GetForcedGamemode();
    void UpdateForceGamemode(Guid? gameId);
    
    Guid? GetCurrentGamemode();
    void UpdateCurrentGamemode(Guid? gameId, JsonElement? settings);
    
    IEnumerable<ButtonDto> GetButtons();
    void UpdateButton(string buttonId, float? distance);
    
    IEnumerable<Guid> GetAvailableGamemodes();
    bool TryAddAvailable(Guid gameId);
    bool TryRemoveAvailable(Guid gameId);
    
    LightstripSettingsDto GetLightstripSettings();
    LightstripResultDto AddLightStrip(bool samePiece, bool onePiece);
    bool TryRemoveLightstrip(int lightId);
    bool UpdateLightStrip(int lightId, LightstripRequestsDto lightstrip);
    
    Task<ContentDto?> GetContentAsync(Guid gameId);
}