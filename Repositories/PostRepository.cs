using BlogCommunityApi.Data;
using BlogCommunityApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogCommunityApi.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _db;
        public PostRepository(AppDbContext db) => _db = db;

        // Read
        public async Task<List<object>> GetAllAsync()
        {
            return await _db.Posts
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
                    Category = new { p.CategoryId, Name = p.Category!.Name },
                    Author = new { p.UserId, Username = p.User!.Username }
                })
                .Cast<object>()
                .ToListAsync();
        }

        public async Task<List<object>> SearchByTitleAsync(string title)
        {
            return await _db.Posts
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
                    Category = new { p.CategoryId, Name = p.Category!.Name },
                    Author = new { p.UserId, Username = p.User!.Username }
                })
                .Cast<object>()
                .ToListAsync();
        }

        public Task<bool> CategoryExistsAsync(int categoryId)
            => _db.Categories.AnyAsync(c => c.Id == categoryId);

        public async Task<List<object>> GetByCategoryAsync(int categoryId)
        {
            return await _db.Posts
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
                    Category = new { p.CategoryId, Name = p.Category!.Name },
                    Author = new { p.UserId, Username = p.User!.Username }
                })
                .Cast<object>()
                .ToListAsync();
        }

        // Write
        public Task<Post?> GetPostEntityByIdAsync(int id)
            => _db.Posts.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<bool> CategoryValidAsync(int categoryId)
            => await _db.Categories.FindAsync(categoryId) != null;

        public async Task<Post> AddAsync(Post post)
        {
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            return post;
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();

        public async Task DeleteAsync(Post post)
        {
            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();
        }
    }
}