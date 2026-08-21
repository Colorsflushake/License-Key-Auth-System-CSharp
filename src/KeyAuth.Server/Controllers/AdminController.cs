namespace KeyAuth.Server.Controllers;

using Microsoft.AspNetCore.Mvc;
using KeyAuth.Server.Services;
using KeyAuth.Server.Models;

[ApiController]
[Route("api/admin")]
public sealed class AdminController : ControllerBase
{
    private readonly KeyService _keyService;

    public AdminController(KeyService keyService)
    {
        _keyService = keyService;
    }

    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse>> GenerateKey([FromBody] GenerateRequest request)
    {
        var license = await _keyService.GenerateAsync(request.Tier, request.Duration, request.MaxDevices);
        return Ok(ApiResponse.Success(license));
    }

    [HttpPost("revoke")]
    public async Task<ActionResult<ApiResponse>> RevokeKey([FromBody] RevokeRequest request)
    {
        var success = await _keyService.RevokeAsync(request.LicenseKey);
        return Ok(success ? ApiResponse.Success("Revoked") : ApiResponse.Fail("Key not found"));
    }

    [HttpGet("stats")]
    public ActionResult<ApiResponse> GetStats()
    {
        var stats = _keyService.GetStatistics();
        return Ok(ApiResponse.Success(stats));
    }

    [HttpPost("reset-hwid")]
    public async Task<ActionResult<ApiResponse>> ResetHwid([FromBody] ResetHwidRequest request)
    {
        var success = await _keyService.ResetHwidAsync(request.LicenseKey);
        return Ok(success ? ApiResponse.Success("HWID reset") : ApiResponse.Fail("Key not found"));
    }
}

public sealed record GenerateRequest(string Tier, TimeSpan Duration, int MaxDevices);
public sealed record RevokeRequest(string LicenseKey);
public sealed record ResetHwidRequest(string LicenseKey);
