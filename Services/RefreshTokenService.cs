using System.Security.Cryptography;

namespace BlogCommunityApi.Services;

public class RefreshTokenService
{
    public string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}