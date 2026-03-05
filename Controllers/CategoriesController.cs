using BlogCommunityApi.Data;
using BlogCommunityApi.DTOs;
using BlogCommunityApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogCommunityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;
    public CategoriesController(AppDbContext db) => _db = db;

    // GET /api/Categories
    // [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new { c.Id, c.Name })
            .ToListAsync();

        return Ok(categories);
    }

     // POST /api/Categories
    // [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        if (dto is null) return BadRequest("Request body is required.");
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required.");

        var name = dto.Name.Trim();
        var exists = await _db.Categories.AnyAsync(c => c.Name == name);
        if (exists) return Conflict("Category already exists.");

        var category = new Category { Name = name };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = category.Id }, new { category.Id, category.Name });
    }
}