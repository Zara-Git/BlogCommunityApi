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
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid token claims.");

        var (ok, statusCode, error, result) = await _service.CreateAsync(userId, req);

        if (!ok) return StatusCode(statusCode, error);

        return CreatedAtAction(nameof(GetByPost), new { postId = req.PostId }, result);
    }

    [HttpGet("by-post/{postId:int}")]
    public async Task<IActionResult> GetByPost([FromRoute] int postId)
    {
        var comments = await _service.GetByPostAsync(postId);
        return Ok(comments);
    }
}