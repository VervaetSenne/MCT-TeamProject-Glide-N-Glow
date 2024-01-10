namespace GlideNGlow.Services.Abstractions;

public interface IAdminService
{
    Task<bool> SetAvailableGamemodesAsync(IEnumerable<Guid> ids);
}