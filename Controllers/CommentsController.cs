using BlogCommunityApi.DTOs;
using BlogCommunityApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogCommunityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _service;
    public CommentsController(ICommentService service) => _service = service;

    // Hämtar userId från JWT-claims (returnerar false om claim saknas/fel)
    private bool TryGetUserId(out int userId)
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        return int.TryParse(idStr, out userId);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentRequest req)
    {
        // Body måste finnas
        if (req is null) return BadRequest("Request body is required.");

        // Validering av DTO (om DataAnnotations finns)
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        // Kräver giltig token med userId
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid token claims.");

        // Business logic i service (inkl. "inte kommentera egen post")
        var (ok, statusCode, error, result) = await _service.CreateAsync(userId, req);

        if (!ok) return StatusCode(statusCode, error);

        // 201 Created + pekar på GET by-post/{postId}
        return CreatedAtAction(nameof(GetByPost), new { postId = req.PostId }, result);
    }

    [HttpGet("by-post/{postId:int}")]
    public async Task<IActionResult> GetByPost([FromRoute] int postId)
    {
        // Returnerar alla kommentarer för ett inlägg (public endpoint)
        var comments = await _service.GetByPostAsync(postId);
        return Ok(comments);
    }
}