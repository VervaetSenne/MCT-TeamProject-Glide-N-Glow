using GlideNGlow.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace GlideNGlow.Ui.WepApi.Controllers;

[Route("[controller]")]
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

        return new OkResult();
    }
}