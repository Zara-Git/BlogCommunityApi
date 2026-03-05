using System.ComponentModel.DataAnnotations;

namespace BlogCommunityApi.DTOs;

public class UpdateUserRequest
{
    [Display(Name = "Username")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Username can only contain letters, numbers, dot, underscore and hyphen.")]
    public string? Username { get; set; }

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
    public string? Email { get; set; }

    [Display(Name = "NewPassword")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
    //  minst en bokstav och en siffra
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{6,}$", ErrorMessage = "Password must contain at least one letter and one number.")]
    public string? NewPassword { get; set; }
}