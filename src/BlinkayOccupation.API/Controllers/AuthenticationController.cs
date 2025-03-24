using BlinkayOccupation.Application.Models;
using BlinkayOccupation.Application.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlinkayOccupation.API.Controllers;

[ApiController]
[Route("api")]
public class AuthenticationController : ControllerBase
{

    private readonly ILogger<AuthenticationController> _logger;
    private readonly IAuthService _authService;

    public AuthenticationController(ILogger<AuthenticationController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var token = await _authService.LoginAsync(request);
        if (token == null)
            return Unauthorized(new { Message = "Invalid credentials" });

        Response.Headers.Append("Expires", token.Expires.ToString("R"));
        return Ok(new { token.Token });
    }
}
