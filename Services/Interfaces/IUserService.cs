using BlogCommunityApi.DTOs;
using System.Threading.Tasks;

namespace BlogCommunityApi.Services.Interfaces
{
    // Service-kontrakt: business logic (regler + validering)
    public interface IUserService
    {
        // Uppdaterar inloggad användare (username/email/password)
        Task<(bool ok, int status, string? error, object? result)> UpdateMeAsync(int userId, UpdateUserRequest dto);

        // Tar bort inloggad användare (inkl. relaterad data)
        Task<(bool ok, int status, string? error)> DeleteMeAsync(int userId);
    }
}