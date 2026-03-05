using BlogCommunityApi.DTOs;
using BlogCommunityApi.Models;
using BlogCommunityApi.Repositories;
using BlogCommunityApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace BlogCommunityApi.Services
{
    // Service: implementerar regler och använder repo för databasen
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        public UserService(IUserRepository repo) => _repo = repo;

        public async Task<(bool ok, int status, string? error, object? result)> UpdateMeAsync(int userId, UpdateUserRequest dto)
        {
            // Hämtar användaren
            var user = await _repo.GetByIdAsync(userId);
            if (user == null) return (false, 404, "User not found.", null);

            // Uppdaterar username om det finns + kollar duplicat
            if (!string.IsNullOrWhiteSpace(dto.Username))
            {
                var newUsername = dto.Username.Trim();
                if (await _repo.UsernameTakenAsync(newUsername, userId))
                    return (false, 409, "Username already exists.", null);

                user.Username = newUsername;
            }

            // Uppdaterar email om det finns + kollar duplicat
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var newEmail = dto.Email.Trim().ToLower();
                if (await _repo.EmailTakenAsync(newEmail, userId))
                    return (false, 409, "Email already exists.", null);

                user.Email = newEmail;
            }

            // Uppdaterar lösenord (hashar alltid)
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                var hasher = new PasswordHasher<User>();
                user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);
            }

            await _repo.SaveChangesAsync();
            return (true, 200, null, new { user.Id, user.Username, user.Email });
        }

        public async Task<(bool ok, int status, string? error)> DeleteMeAsync(int userId)
        {
            // Hämtar användaren med relationer (för delete)
            var user = await _repo.GetUserWithRelationsAsync(userId);
            if (user == null) return (false, 404, "User not found.");

            await _repo.DeleteUserWithRelationsAsync(user);
            return (true, 204, null);
        }
    }
}