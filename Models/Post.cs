using System.ComponentModel.DataAnnotations;

namespace BlogCommunityApi.Models;

// Entity/Model för tabellen "Posts"
public class Post
{
    // Primärnyckel (PK)
    public int Id { get; set; }

    // Titel: krävs + maxlängd
    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    // Text: krävs + maxlängd (för att skydda DB från extremt långa texter)
    [Required]
    [MaxLength(5000)]
    public string Text { get; set; } = string.Empty;

    // Skapad tidpunkt (UTC) - kan sättas i service vid skapande
    public DateTime CreatedAt { get; set; }

    // FK: författare (User)
    public int UserId { get; set; }
    public User? User { get; set; }

    // FK: kategori (Category)
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // Navigation: kommentarer för detta inlägg
    public List<Comment> Comments { get; set; } = new();
}