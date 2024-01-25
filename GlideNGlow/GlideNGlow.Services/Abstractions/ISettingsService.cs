using GlideNGlow.Core.Dto;

namespace GlideNGlow.Services.Abstractions;

public interface ISettingsService
{
    bool GetAllowSwitching();
    bool UpdateAllowSwitching(bool value);
    
    Guid? GetForcedGamemode();
    void UpdateForceGamemode(Guid? gameId);
    
    Guid? GetCurrentGamemode();
    void UpdateCurrentGamemode(Guid? gameId);
    
    IEnumerable<ButtonDto> GetButtons();
    void UpdateButton(string buttonId, float? distance);
    
    IEnumerable<Guid> GetAvailableGamemodes();
    bool TryAddAvailable(Guid gameId);
    bool TryRemoveAvailable(Guid gameId);
    
    LightstripSettingsDto GetLightstripSettings();
    LightstripDto AddLightStrip(bool samePiece, bool onePiece);
    bool TryRemoveLightstrip(int lightId);
    bool UpdateLightStrip(int lightId, LightstripDto lightstrip);
}