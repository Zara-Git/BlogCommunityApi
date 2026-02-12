using System.ComponentModel.DataAnnotations;

namespace BlogCommunityApi.DTOs;

public class RegisterRequest
{
    [Required, MaxLength(50)]
    public string Username { get; set; } = "";

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = "";

    [Required, MinLength(6)]
    public string Password { get; set; } = "";
}

public class LoginRequest
{
    [Required]
    public string Username { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public class AuthResponse
{
    public string Token { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = "";
}

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = "";
}

public class UpdateProfileRequest
{
    [MaxLength(50)]
    public string? Username { get; set; }

    [EmailAddress, MaxLength(100)]
    public string? Email { get; set; }

    [MinLength(6)]
    public string? NewPassword { get; set; }
}