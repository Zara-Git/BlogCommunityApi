using System.ComponentModel.DataAnnotations;

namespace BlogCommunityApi.Models;

// Entity/Model för tabellen "Comments"
// Beskriver en kommentar och kopplingen till Post och User.
public class Comment
{
    // Primärnyckel (PK)
    public int Id { get; set; }

    // Kommentartext (krävs + maxlängd)
    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    // Skapad tidpunkt (sätts i service vid skapande)
    public DateTime CreatedAt { get; set; }

    // FK: vilket inlägg kommentaren tillhör
    public int PostId { get; set; }
    public Post? Post { get; set; }

    // FK: vilken användare som skrev kommentaren
    public int UserId { get; set; }
    public User? User { get; set; }
}