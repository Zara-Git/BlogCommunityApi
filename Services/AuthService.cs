using BlogCommunityApi.DTOs;
using BlogCommunityApi.Models;
using BlogCommunityApi.Repositories;
using BlogCommunityApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace BlogCommunityApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly JwtService _jwt;

        public AuthService(IUserRepository users, JwtService jwt)
        {
            _users = users;
            _jwt = jwt;
        }

        public async Task<(bool ok, string? error, object? result)> RegisterAsync(RegisterRequest req)
        {
            var username = req.Username.Trim();
            var email = req.Email.Trim().ToLower();

            if (await _users.UsernameExistsAsync(username))
                return (false, "Username already exists.", null);

            if (await _users.EmailExistsAsync(email))
                return (false, "Email already exists.", null);

            var user = new User { Username = username, Email = email };

            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, req.Password);

            await _users.AddAsync(user);

            var response = new { userId = user.Id, user.Username, user.Email };
            return (true, null, response);
        }

        public async Task<(bool ok, string? error, AuthResponse? result)> LoginAsync(LoginRequest req)
        {
            var user = await _users.GetByUsernameAsync(req.Username);
            if (user is null)
                return (false, "Invalid username or password.", null);

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);

            if (result == PasswordVerificationResult.Failed)
                return (false, "Invalid username or password.", null);

            var token = _jwt.CreateToken(user.Id, user.Email);

            return (true, null, new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });
        }
    }
}