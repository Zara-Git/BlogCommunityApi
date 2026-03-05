using BlogCommunityApi.DTOs;
using BlogCommunityApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogCommunityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _service;
    public PostsController(IPostService service) => _service = service;

    // Hämtar userId från JWT-claims (returnerar false om claim saknas/fel)
    private bool TryGetUserId(out int userId)
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        return int.TryParse(idStr, out userId);
    }

    // Public: läsa alla inlägg
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    // Public: sök på titel
    [HttpGet("search")]
    public async Task<IActionResult> SearchByTitle([FromQuery] string title)
    {
        var (ok, status, error, result) = await _service.SearchByTitleAsync(title);
        if (!ok) return StatusCode(status, error);
        return Ok(result);
    }

    // Public: filtrera på kategori
    [HttpGet("by-category/{categoryId:int}")]
    public async Task<IActionResult> ByCategory([FromRoute] int categoryId)
    {
        var (ok, status, error, result) = await _service.ByCategoryAsync(categoryId);
        if (!ok) return StatusCode(status, error);
        return Ok(result);
    }

    // Inloggad: skapa inlägg
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest req)
    {
        if (req is null) return BadRequest("Request body is required.");
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token claims.");

        var (ok, status, error, result) = await _service.CreateAsync(userId, req);
        if (!ok) return StatusCode(status, error);

        return CreatedAtAction(nameof(GetAll), new { message = "Post created", result });
    }

    // Inloggad: uppdatera inlägg (endast ägaren ska få göra detta - kontrolleras i service)
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePostRequest req)
    {
        if (req is null) return BadRequest("Request body is required.");
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token claims.");

        var (ok, status, error, _) = await _service.UpdateAsync(userId, id, req);
        if (!ok) return StatusCode(status, error);

        return Ok(new { message = "Post updated", id });
    }

    // Inloggad: ta bort inlägg (endast ägaren ska få göra detta - kontrolleras i service)
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token claims.");

        var (ok, status, error) = await _service.DeleteAsync(userId, id);
        if (!ok) return StatusCode(status, error);

        return NoContent(); // 204
    }
}