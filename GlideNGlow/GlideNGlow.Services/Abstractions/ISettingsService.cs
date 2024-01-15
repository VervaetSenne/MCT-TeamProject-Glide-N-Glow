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
}