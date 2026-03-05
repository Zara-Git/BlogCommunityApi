using BlogCommunityApi.Data;
using BlogCommunityApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogCommunityApi.Repositories
{
    // Repository: DB-access via EF Core (AppDbContext)
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) => _db = db;

        //  Auth 

        // Kollar om username redan finns
        public Task<bool> UsernameExistsAsync(string username)
            => _db.Users.AsNoTracking().AnyAsync(u => u.Username == username);

        // Kollar om email redan finns
        public Task<bool> EmailExistsAsync(string email)
            => _db.Users.AsNoTracking().AnyAsync(u => u.Email == email);

        // Hämtar user för login (via username)
        public Task<User?> GetByUsernameAsync(string username)
            => _db.Users.FirstOrDefaultAsync(u => u.Username == username);

        // Skapar ny user och sparar i databasen
        public async Task<User> AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        //  Update 

        // Hämtar user för uppdatering (tracking behövs)
        public Task<User?> GetByIdAsync(int id)
            => _db.Users.FirstOrDefaultAsync(u => u.Id == id);

        // Kollar om nytt username är upptaget (exkluderar aktuell user)
        public Task<bool> UsernameTakenAsync(string username, int excludeUserId)
            => _db.Users.AsNoTracking().AnyAsync(u => u.Username == username && u.Id != excludeUserId);

        // Kollar om nytt email är upptaget (exkluderar aktuell user)
        public Task<bool> EmailTakenAsync(string email, int excludeUserId)
            => _db.Users.AsNoTracking().AnyAsync(u => u.Email == email && u.Id != excludeUserId);

        // Sparar ändringar i databasen
        public Task SaveChangesAsync()
            => _db.SaveChangesAsync();

        //  Delete 

        // Hämtar user + relationer för att kunna ta bort utan FK-problem
        public Task<User?> GetUserWithRelationsAsync(int id)
            => _db.Users
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == id);

        // Tar bort user + relaterad data (Posts/Comments/Tokens)
        public async Task DeleteUserWithRelationsAsync(User user)
        {
            _db.Comments.RemoveRange(user.Comments);
            _db.Posts.RemoveRange(user.Posts);
            _db.RefreshTokens.RemoveRange(user.RefreshTokens);

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }
}