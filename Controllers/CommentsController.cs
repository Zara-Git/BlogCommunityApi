using BlogCommunityApi.Data;
using BlogCommunityApi.DTOs;
using BlogCommunityApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogCommunityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public CommentsController(AppDbContext db) => _db = db;

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
        if (req is null) return BadRequest("Request body is required.");
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token claims.");
        if (string.IsNullOrWhiteSpace(req.Text)) return BadRequest("Comment text is required.");

        var post = await _db.Posts.AsNoTracking().FirstOrDefaultAsync(post => post.Id == req.PostId);
        if (post is null) return NotFound("Post not found.");

        if (post.UserId == userId)
            return BadRequest("You cannot comment on your own post.");

        var comment = new Comment
        {
            Text = req.Text.Trim(),
            PostId = req.PostId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByPost), new { postId = comment.PostId }, new { comment.Id });
    }

    [HttpGet("by-post/{postId:int}")]
    public async Task<IActionResult> GetByPost([FromRoute] int postId)
    {
        var comments = await _db.Comments
            .AsNoTracking()
            .Include(comment => comment.User)
            .Where(comment => comment.PostId == postId)
            .OrderByDescending(comment => comment.CreatedAt)
            .Select(comment => new
            {
                comment.Id,
                comment.Text,
                comment.CreatedAt,
                Author = new { comment.UserId, comment.User!.Username },
                comment.PostId
            })
            .ToListAsync();

        return Ok(comments);
    }
}