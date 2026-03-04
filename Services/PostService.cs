using BlogCommunityApi.DTOs;
using BlogCommunityApi.Models;
using BlogCommunityApi.Repositories;
using BlogCommunityApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogCommunityApi.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _repo;
        public PostService(IPostRepository repo) => _repo = repo;

        // Public read
        public Task<List<object>> GetAllAsync()
            => _repo.GetAllAsync();

        public async Task<(bool ok, int status, string? error, List<object>? result)> SearchByTitleAsync(string title)
        {
            title = (title ?? "").Trim();
            if (title.Length == 0) return (false, 400, "title is required.", null);

            var posts = await _repo.SearchByTitleAsync(title);
            return (true, 200, null, posts);
        }

        public async Task<(bool ok, int status, string? error, List<object>? result)> ByCategoryAsync(int categoryId)
        {
            var exists = await _repo.CategoryExistsAsync(categoryId);
            if (!exists) return (false, 404, "Category not found.", null);

            var posts = await _repo.GetByCategoryAsync(categoryId);
            return (true, 200, null, posts);
        }

        // Write
        public async Task<(bool ok, int status, string? error, object? result)> CreateAsync(int userId, CreatePostRequest req)
        {
            if (req is null) return (false, 400, "Request body is required.", null);
            if (string.IsNullOrWhiteSpace(req.Title) || string.IsNullOrWhiteSpace(req.Text))
                return (false, 400, "Title and text are required.", null);

            var categoryValid = await _repo.CategoryValidAsync(req.CategoryId);
            if (!categoryValid) return (false, 400, "Invalid categoryId.", null);

            var post = new Post
            {
                Title = req.Title.Trim(),
                Text = req.Text.Trim(),
                UserId = userId,
                CategoryId = req.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(post);
            return (true, 201, null, new { post.Id });
        }

        public async Task<(bool ok, int status, string? error, object? result)> UpdateAsync(int userId, int id, UpdatePostRequest req)
        {
            if (req is null) return (false, 400, "Request body is required.", null);
            if (string.IsNullOrWhiteSpace(req.Title) || string.IsNullOrWhiteSpace(req.Text))
                return (false, 400, "Title and text are required.", null);

            var post = await _repo.GetPostEntityByIdAsync(id);
            if (post is null) return (false, 404, "Post not found.", null);

            if (post.UserId != userId)
                return (false, 403, "Only the author can update this post.", null);

            var categoryValid = await _repo.CategoryValidAsync(req.CategoryId);
            if (!categoryValid) return (false, 400, "Invalid categoryId.", null);

            post.Title = req.Title.Trim();
            post.Text = req.Text.Trim();
            post.CategoryId = req.CategoryId;

            await _repo.SaveChangesAsync();
            return (true, 200, null, new { post.Id });
        }

        public async Task<(bool ok, int status, string? error)> DeleteAsync(int userId, int id)
        {
            var post = await _repo.GetPostEntityByIdAsync(id);
            if (post is null) return (false, 404, "Post not found.");

            if (post.UserId != userId)
                return (false, 403, "Only the author can delete this post.");

            await _repo.DeleteAsync(post);
            return (true, 204, null);
        }
    }
}