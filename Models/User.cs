using System.ComponentModel.DataAnnotations;

namespace BlogCommunityApi.Models;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; } = "";

    [Required, MaxLength(100)]
    public string Email { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";

    public List<Post> Posts { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<RefreshToken> RefreshTokens { get; set; } = new();

    // Summary: Represents a user entity with basic
    // identity/auth fields (username, email, password hash)
    // and navigation collections that define one-to-many
    // relationships to posts, comments, and refresh tokens in the database.
}