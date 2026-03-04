using BlogCommunityApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogCommunityApi.Services.Interfaces
{
    public interface ICommentService
    {
        Task<(bool ok, int statusCode, string? error, object? result)> CreateAsync(int userId, CreateCommentRequest req);
        Task<List<object>> GetByPostAsync(int postId);
    }
}