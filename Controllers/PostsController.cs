using BlogCommunityApi.DTOs;
using BlogCommunityApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogCommunityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _service;
    public PostsController(IPostService service) => _service = service;

    private bool TryGetUserId(out int userId)
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        return int.TryParse(idStr, out userId);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("search")]
    public async Task<IActionResult> SearchByTitle([FromQuery] string title)
    {
        var (ok, status, error, result) = await _service.SearchByTitleAsync(title);
        if (!ok) return StatusCode(status, error);
        return Ok(result);
    }

    [HttpGet("by-category/{categoryId:int}")]
    public async Task<IActionResult> ByCategory([FromRoute] int categoryId)
    {
        var (ok, status, error, result) = await _service.ByCategoryAsync(categoryId);
        if (!ok) return StatusCode(status, error);
        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token claims.");

        var (ok, status, error, result) = await _service.CreateAsync(userId, req);
        if (!ok) return StatusCode(status, error);

        return CreatedAtAction(nameof(GetAll), result);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePostRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token claims.");

        var (ok, status, error, result) = await _service.UpdateAsync(userId, id, req);
        if (!ok) return StatusCode(status, error);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token claims.");

        var (ok, status, error) = await _service.DeleteAsync(userId, id);
        if (!ok) return StatusCode(status, error);

        return NoContent();
    }
}