using BlogCommunityApi.DTOs;
using BlogCommunityApi.Models;
using BlogCommunityApi.Repositories;
using BlogCommunityApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogCommunityApi.Services
{
    // Service: business logic för Posts (validering + ägarkontroll)
    public class PostService : IPostService
    {
        private readonly IPostRepository _repo;
        public PostService(IPostRepository repo) => _repo = repo;

        // Public: hämtar alla inlägg (ingen inlogg krävs)
        public Task<List<object>> GetAllAsync()
            => _repo.GetAllAsync();

        // Public: sök på titel
        public async Task<(bool ok, int status, string? error, List<object>? result)> SearchByTitleAsync(string title)
        {
            title = (title ?? "").Trim();
            if (title.Length == 0)
                return (false, 400, "Title is required.", null);

            var posts = await _repo.SearchByTitleAsync(title);
            return (true, 200, null, posts);
        }

        // Public: filtrera på kategori
        public async Task<(bool ok, int status, string? error, List<object>? result)> ByCategoryAsync(int categoryId)
        {
            if (categoryId <= 0)
                return (false, 400, "CategoryId must be greater than 0.", null);

            var exists = await _repo.CategoryExistsAsync(categoryId);
            if (!exists)
                return (false, 404, "Category not found.", null);

            var posts = await _repo.GetByCategoryAsync(categoryId);
            return (true, 200, null, posts);
        }

        // Inloggad: skapa inlägg (kopplas till userId)
        public async Task<(bool ok, int status, string? error, object? result)> CreateAsync(int userId, CreatePostRequest req)
        {
            if (req is null) return (false, 400, "Request body is required.", null);

            var title = (req.Title ?? "").Trim();
            var text = (req.Text ?? "").Trim();

            if (title.Length == 0 || text.Length == 0)
                return (false, 400, "Title and text are required.", null);

            if (req.CategoryId <= 0)
                return (false, 400, "CategoryId must be greater than 0.", null);

            var categoryValid = await _repo.CategoryValidAsync(req.CategoryId);
            if (!categoryValid)
                return (false, 404, "Category not found.", null);

            var post = new Post
            {
                Title = title,
                Text = text,
                UserId = userId,
                CategoryId = req.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(post);

            // Enkel respons
            return (true, 201, null, new { message = "Post created", id = post.Id });
        }

        // Inloggad: uppdatera inlägg (endast ägaren)
        public async Task<(bool ok, int status, string? error, object? result)> UpdateAsync(int userId, int id, UpdatePostRequest req)
        {
            if (req is null) return (false, 400, "Request body is required.", null);

            var title = (req.Title ?? "").Trim();
            var text = (req.Text ?? "").Trim();

            if (title.Length == 0 || text.Length == 0)
                return (false, 400, "Title and text are required.", null);

            if (req.CategoryId <= 0)
                return (false, 400, "CategoryId must be greater than 0.", null);

            var post = await _repo.GetPostEntityByIdAsync(id);
            if (post is null)
                return (false, 404, "Post not found.", null);

            // Ägarkontroll
            if (post.UserId != userId)
                return (false, 403, "Only the author can update this post.", null);

            var categoryValid = await _repo.CategoryValidAsync(req.CategoryId);
            if (!categoryValid)
                return (false, 404, "Category not found.", null);

            post.Title = title;
            post.Text = text;
            post.CategoryId = req.CategoryId;

            await _repo.SaveChangesAsync();

            return (true, 200, null, new { message = "Post updated", id = post.Id });
        }

        // Inloggad: ta bort inlägg (endast ägaren)
        public async Task<(bool ok, int status, string? error)> DeleteAsync(int userId, int id)
        {
            var post = await _repo.GetPostEntityByIdAsync(id);
            if (post is null)
                return (false, 404, "Post not found.");

            // Ägarkontroll
            if (post.UserId != userId)
                return (false, 403, "Only the author can delete this post.");

            await _repo.DeleteAsync(post);
            return (true, 204, null);
        }
    }
}