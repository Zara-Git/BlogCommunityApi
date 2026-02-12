using BlogCommunityApi.Data;
using BlogCommunityApi.DTOs;
using BlogCommunityApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogCommunityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db) => _db = db;

    private int GetUserId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        if (!int.TryParse(idStr, out var userId))
            throw new UnauthorizedAccessException("Invalid token claims.");

        return userId;
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserRequest dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var userId = GetUserId();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound("User not found.");

        if (!string.IsNullOrWhiteSpace(dto.Username))
        {
            var newUsername = dto.Username.Trim();
            var usernameTaken = await _db.Users.AnyAsync(u => u.Username == newUsername && u.Id != userId);
            if (usernameTaken) return Conflict("Username already exists.");
            user.Username = newUsername;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var newEmail = dto.Email.Trim().ToLower();
            var emailTaken = await _db.Users.AnyAsync(u => u.Email == newEmail && u.Id != userId);
            if (emailTaken) return Conflict("Email already exists.");
            user.Email = newEmail;
        }

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);
        }

        await _db.SaveChangesAsync();
        return Ok(new { user.Id, user.Username, user.Email });
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        var userId = GetUserId();

        var user = await _db.Users
            .Include(u => u.Posts)
            .Include(u => u.Comments)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return NotFound("User not found.");

        // Avoid FK issues with Restrict rules
        _db.Comments.RemoveRange(user.Comments);
        _db.Posts.RemoveRange(user.Posts);
        _db.RefreshTokens.RemoveRange(user.RefreshTokens);

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}