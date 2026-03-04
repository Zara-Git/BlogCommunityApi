using BlogCommunityApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogCommunityApi.Repositories
{
    public interface IPostRepository
    {
        // Read
        Task<List<object>> GetAllAsync();
        Task<List<object>> SearchByTitleAsync(string title);
        Task<bool> CategoryExistsAsync(int categoryId);
        Task<List<object>> GetByCategoryAsync(int categoryId);

        // Write
        Task<Post?> GetPostEntityByIdAsync(int id);
        Task<bool> CategoryValidAsync(int categoryId);
        Task<Post> AddAsync(Post post);
        Task SaveChangesAsync();
        Task DeleteAsync(Post post);
    }
}