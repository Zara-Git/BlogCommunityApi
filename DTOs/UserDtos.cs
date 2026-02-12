using System.ComponentModel.DataAnnotations;

namespace BlogCommunityApi.DTOs;

public class UpdateUserRequest
{
    [MaxLength(50)]
    public string? Username { get; set; }

    [EmailAddress, MaxLength(100)]
    public string? Email { get; set; }

    [MinLength(6)]
    public string? NewPassword { get; set; }
}