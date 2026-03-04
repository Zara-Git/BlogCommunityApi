using BlogCommunityApi.DTOs;
using BlogCommunityApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogCommunityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var (ok, error, result) = await _auth.RegisterAsync(req);
        if (!ok) return Conflict(error);

        // CreatedAtAction 
        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var (ok, error, result) = await _auth.LoginAsync(req);
        if (!ok) return Unauthorized(error);

        return Ok(result);
    }
}