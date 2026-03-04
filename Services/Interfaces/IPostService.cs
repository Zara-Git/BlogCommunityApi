using BlogCommunityApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogCommunityApi.Services.Interfaces
{
    public interface IPostService
    {
        // Public read
        Task<List<object>> GetAllAsync();
        Task<(bool ok, int status, string? error, List<object>? result)> SearchByTitleAsync(string title);
        Task<(bool ok, int status, string? error, List<object>? result)> ByCategoryAsync(int categoryId);

        // Write
        Task<(bool ok, int status, string? error, object? result)> CreateAsync(int userId, CreatePostRequest req);
        Task<(bool ok, int status, string? error, object? result)> UpdateAsync(int userId, int id, UpdatePostRequest req);
        Task<(bool ok, int status, string? error)> DeleteAsync(int userId, int id);
    }
}