using BlogCommunityApi.DTOs;
using System.Threading.Tasks;

namespace BlogCommunityApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool ok, string? error, object? result)> RegisterAsync(RegisterRequest req);
        Task<(bool ok, string? error, AuthResponse? result)> LoginAsync(LoginRequest req);
    }
}