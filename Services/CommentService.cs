using BlogCommunityApi.DTOs;
using BlogCommunityApi.Models;
using BlogCommunityApi.Repositories;
using BlogCommunityApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogCommunityApi.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repo;

        public CommentService(ICommentRepository repo) => _repo = repo;

        public async Task<(bool ok, int statusCode, string? error, object? result)> CreateAsync(int userId, CreateCommentRequest req)
        {
            if (req is null) return (false, 400, "Request body is required.", null);
            if (string.IsNullOrWhiteSpace(req.Text)) return (false, 400, "Comment text is required.", null);

            var post = await _repo.GetPostByIdAsync(req.PostId);
            if (post is null) return (false, 404, "Post not found.", null);

            if (post.UserId == userId)
                return (false, 400, "You cannot comment on your own post.", null);

            var comment = new Comment
            {
                Text = req.Text.Trim(),
                PostId = req.PostId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(comment);

           
            var result = new { comment.Id };
            return (true, 201, null, result);
        }

        public Task<List<object>> GetByPostAsync(int postId)
            => _repo.GetByPostAsync(postId);
    }
}