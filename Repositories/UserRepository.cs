using BlogCommunityApi.Data;
using BlogCommunityApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlogCommunityApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db) => _db = db;

        public Task<bool> UsernameExistsAsync(string username) =>
            _db.Users.AnyAsync(u => u.Username == username);

        public Task<bool> EmailExistsAsync(string email) =>
            _db.Users.AnyAsync(u => u.Email == email);

        public Task<User?> GetByUsernameAsync(string username) =>
            _db.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User> AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }
}