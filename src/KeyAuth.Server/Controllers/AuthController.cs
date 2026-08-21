namespace KeyAuth.Server.Controllers;

using Microsoft.AspNetCore.Mvc;
using KeyAuth.Server.Services;
using KeyAuth.Server.Models;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly KeyService _keyService;
    private readonly HwidService _hwidService;

    public AuthController(KeyService keyService, HwidService hwidService)
    {
        _keyService = keyService;
        _hwidService = hwidService;
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ApiResponse>> Validate([FromForm] string key, [FromForm] string hwid, [FromForm] string app_id)
    {
        var license = await _keyService.FindByKeyAsync(key);
        if (license is null)
            return Ok(ApiResponse.Fail("Invalid license key"));

        if (license.ExpiresAt < DateTime.UtcNow)
            return Ok(ApiResponse.Fail("License expired"));

        if (!string.IsNullOrEmpty(license.BoundHwid) && license.BoundHwid != hwid)
            return Ok(ApiResponse.Fail("HWID mismatch"));

        if (string.IsNullOrEmpty(license.BoundHwid))
        {
            await _hwidService.BindAsync(license.Id, hwid);
        }

        return Ok(ApiResponse.Success(license));
    }

    [HttpPost("heartbeat")]
    public ActionResult<ApiResponse> Heartbeat([FromForm] string session_id, [FromForm] string hwid)
    {
        var valid = _hwidService.ValidateSession(session_id, hwid);
        return Ok(valid ? ApiResponse.Success("alive") : ApiResponse.Fail("Invalid session"));
    }
}
