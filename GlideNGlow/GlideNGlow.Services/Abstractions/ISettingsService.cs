using GlideNGlow.Core.Dto;

namespace GlideNGlow.Services.Abstractions;

public interface ISettingsService
{
    bool UpdateAllowSwitching(bool value);
    IEnumerable<Guid> GetAvailableGamemodes();
    Guid? GetForcedGamemode();
    bool GetAllowSwitching();
    bool TryAddAvailable(Guid gameId);
    bool TryRemoveAvailable(Guid gameId);
    void UpdateForceGamemode(Guid? gameId);
    void UpdateCurrentGamemode(Guid? gameId);
    IEnumerable<ButtonDto> GetButtons();
    void UpdateButton(string buttonId, float? distance);
    LightstripSettingsDto GetLightstripSettings();
    LightstripDto AddLightStrip(bool samePiece, bool onePiece);
    bool TryRemoveLightstrip(int lightId);
    bool UpdateLightStrip(int lightId, LightstripDto lightstrip);
}