using GlideNGlow.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("admin")]
[ApiController]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }
    
    [HttpPost("set-available")]
    public async Task<IActionResult> SetAvailableAsync([FromBody] List<Guid> games)
    {
        var result = await _adminService.SetAvailableGamemodesAsync(games);
        if (result)
            return new BadRequestResult();

        return Ok();
    }

    [HttpPost("allow-game-switch/{value:bool}")]
    public IActionResult AllowGameSwitch(bool value)
    {
        _adminService.AllowGameSwitch(value);
        return Ok();
    }
    
    [HttpPost("force-gamemode/{value:bool}")]
    public IActionResult ForceGamemode(bool value)
    {
        _adminService.ForceGamemode(value);
        return Ok();
    }
    
    [HttpPost("lightning/{value:bool}")]
    public IActionResult LightingSwitch(bool value)
    {
        _adminService.LightingSwitch(value);
        return Ok();
    }
}