using GlideNGlow.Common.Models.Settings;
using GlideNGlow.Services.Abstractions;
using Microsoft.Extensions.Options.Implementations;

namespace GlideNGlow.Services;

public class AdminService : IAdminService
{
    private readonly IWritableOptions<AppSettings> _appsettings;
    private readonly IUserService _userService;

    private AppSettings AppSettings => _appsettings.CurrentValue;

    public AdminService(IWritableOptions<AppSettings> appsettings, IUserService userService)
    {
        _appsettings = appsettings;
        _userService = userService;
    }

    public async Task<bool> SetAvailableGamemodesAsync(IEnumerable<Guid> ids)
    {
        var gamemodes = await _userService.GetGamemodesAsync();
        var idlist = ids.ToList();
        if (idlist.Any(id => gamemodes.Any(g => g.Id != id)))
            return false; //TODO use service result.
        _appsettings.Update(settings => settings.AvailableGamemodes = idlist);
        // TODO continue with massimo code
        return true;
    }

    public void AllowGameSwitch(bool value)
    {
        _appsettings.Update(settings => settings.ForceGamemode = value);
    }

    public void ForceGamemode(bool value)
    {
        _appsettings.Update(settings => settings.ForceGamemode = value);
    }

    public void LightingSwitch(bool value)
    {
        _appsettings.Update(settings => settings.LightingToggle = value);
    }
}