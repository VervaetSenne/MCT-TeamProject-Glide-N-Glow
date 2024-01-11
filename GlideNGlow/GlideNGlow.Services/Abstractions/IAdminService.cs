namespace GlideNGlow.Services.Abstractions;

public interface IAdminService
{
    Task<bool> SetAvailableGamemodesAsync(IEnumerable<Guid> ids);
    void AllowGameSwitch(bool value);
    void ForceGamemode(bool value);
    void LightingSwitch(bool value);
}