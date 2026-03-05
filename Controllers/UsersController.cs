using BlogCommunityApi.DTOs;
using BlogCommunityApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogCommunityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    public UsersController(IUserService service) => _service = service;

    // Hämtar userId från JWT (claims). Returnerar false om claim saknas/är fel.
    private bool TryGetUserId(out int userId)
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        return int.TryParse(idStr, out userId);
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserRequest dto)
    {
        // Validerar att body finns
        if (dto is null) return BadRequest("Request body is required.");

        // Validerar DTO-regler (om du har DataAnnotations på DTO)
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        // Säkerställer att token har korrekt userId
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid token claims.");

        // Om inget fält skickas, finns inget att uppdatera
        var nothingToUpdate =
            string.IsNullOrWhiteSpace(dto.Username) &&
            string.IsNullOrWhiteSpace(dto.Email) &&
            string.IsNullOrWhiteSpace(dto.NewPassword);

        if (nothingToUpdate)
            return BadRequest("Nothing to update.");

        // Business logic ligger i service-lagret
        var (ok, status, error, result) = await _service.UpdateMeAsync(userId, dto);

        if (!ok) return StatusCode(status, error);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        // Säkerställer att token har korrekt userId
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid token claims.");

        // Tar bort användaren (inkl. relaterad data enligt repo/service)
        var (ok, status, error) = await _service.DeleteMeAsync(userId);

        if (!ok) return StatusCode(status, error);
        return NoContent(); // 204
    }
}