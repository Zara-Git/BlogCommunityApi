using System.ComponentModel.DataAnnotations;

namespace BlogCommunityApi.Models;

public class User
{
    // Primärnyckel (PK) i databasen
    public int Id { get; set; }

    // Användarnamn: krävs + maxlängd (validering + DB-kolumnbegränsning)
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    // Email: krävs + maxlängd (format-validering görs oftast i DTO)
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    // Hashat lösenord (aldrig plain text)
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    // Navigation: 1 användare kan ha många inlägg (1-to-many)
    public List<Post> Posts { get; set; } = new();

    // Navigation: 1 användare kan ha många kommentarer (1-to-many)
    public List<Comment> Comments { get; set; } = new();

    // Navigation: 1 användare kan ha många refresh tokens (1-to-many)
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}