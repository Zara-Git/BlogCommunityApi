using BlogCommunityApi.Data;
using BlogCommunityApi.DTOs;
using BlogCommunityApi.Models;
using BlogCommunityApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogCommunityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext db, JwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var usernameExists = await _db.Users.AnyAsync(u => u.Username == req.Username);
        if (usernameExists) return Conflict("Username already exists.");

        var emailExists = await _db.Users.AnyAsync(u => u.Email == req.Email);
        if (emailExists) return Conflict("Email already exists.");

        var user = new User
        {
            Username = req.Username.Trim(),
            Email = req.Email.Trim().ToLower()
        };

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, req.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new { id = user.Id },
            new { userId = user.Id, user.Username, user.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
        if (user is null) return Unauthorized("Invalid username or password.");

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);

        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid username or password.");

        var token = _jwtService.CreateToken(user.Id, user.Email);

        return Ok(new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }
}