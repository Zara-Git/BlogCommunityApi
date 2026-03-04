using BlogCommunityApi.Data;
using BlogCommunityApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogCommunityApi.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _db;
        public CommentRepository(AppDbContext db) => _db = db;

        public async Task<Post?> GetPostByIdAsync(int postId)
        {
            return await _db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task<Comment> AddAsync(Comment comment)
        {
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();
            return comment;
        }

        public async Task<List<object>> GetByPostAsync(int postId)
        {
            return await _db.Comments
                .AsNoTracking()
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.Id,
                    c.Text,
                    c.CreatedAt,
                    Author = new { c.UserId, Username = c.User!.Username },
                    c.PostId
                })
                .Cast<object>()
                .ToListAsync();
        }
    }
}