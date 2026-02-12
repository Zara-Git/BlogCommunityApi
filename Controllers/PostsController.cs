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
public class PostsController : ControllerBase
{
    private readonly AppDbContext _db;
    public PostsController(AppDbContext db) => _db = db;

    private bool TryGetUserId(out int userId)
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("sub");

        return int.TryParse(idStr, out userId);
    }

    // =========================
    // PUBLIC READ ENDPOINTS
    // =========================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _db.Posts
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Text,
                p.CreatedAt,
                Category = new { p.CategoryId, p.Category!.Name },
                Author = new { p.UserId, p.User!.Username }
            })
            .ToListAsync();

        return Ok(posts);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByTitle([FromQuery] string title)
    {
        title = (title ?? "").Trim();
        if (title.Length == 0) return BadRequest("title is required.");

        var posts = await _db.Posts
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.User)
            .Where(p => p.Title.Contains(title))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Text,
                p.CreatedAt,
                Category = new { p.CategoryId, p.Category!.Name },
                Author = new { p.UserId, p.User!.Username }
            })
            .ToListAsync();

        return Ok(posts);
    }

    [HttpGet("by-category/{categoryId:int}")]
    public async Task<IActionResult> ByCategory([FromRoute] int categoryId)
    {
        var exists = await _db.Categories.AnyAsync(c => c.Id == categoryId);
        if (!exists) return NotFound("Category not found.");

        var posts = await _db.Posts
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.User)
            .Where(p => p.CategoryId == categoryId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Text,
                p.CreatedAt,
                Category = new { p.CategoryId, p.Category!.Name },
                Author = new { p.UserId, p.User!.Username }
            })
            .ToListAsync();

        return Ok(posts);
    }

    // =========================
    // AUTH REQUIRED (WRITE)
    // =========================

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token claims.");

        var category = await _db.Categories.FindAsync(req.CategoryId);
        if (category is null) return BadRequest("Invalid categoryId.");

        var post = new Post
        {
            Title = req.Title.Trim(),
            Text = req.Text.Trim(),
            UserId = userId,
            CategoryId = req.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = post.Id }, new { post.Id });
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePostRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token claims.");

        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == id);
        if (post is null) return NotFound("Post not found.");

        if (post.UserId != userId)
            return StatusCode(StatusCodes.Status403Forbidden, "Only the author can update this post.");

        var category = await _db.Categories.FindAsync(req.CategoryId);
        if (category is null) return BadRequest("Invalid categoryId.");

        post.Title = req.Title.Trim();
        post.Text = req.Text.Trim();
        post.CategoryId = req.CategoryId;

        await _db.SaveChangesAsync();
        return Ok(new { post.Id });
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized("Invalid token claims.");

        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == id);
        if (post is null) return NotFound("Post not found.");

        if (post.UserId != userId)
            return StatusCode(StatusCodes.Status403Forbidden, "Only the author can delete this post.");

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
    
    