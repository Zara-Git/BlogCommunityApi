using System.ComponentModel.DataAnnotations;   // DataAnnotations – validering som [Required], [MaxLength]

namespace BlogCommunityApi.Models;

// Entity/Model för tabellen "Posts"
// Den här klassen beskriver ett blogginlägg och dess relationer till User, Category och Comments.
public class Post
{
    public int Id { get; set; }                         // Primärnyckel (auto-increment i databasen)

    [Required, MaxLength(120)]                          // Titel är obligatorisk + max 120 tecken
    public string Title { get; set; } = "";             // Inläggets titel

    [Required]                                          // Texten är obligatorisk
    public string Text { get; set; } = "";              // Inläggets innehåll (brödtext)

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // När inlägget skapades (UTC). Default sätts när objektet skapas.

    // Koppling till User (författare)
    public int UserId { get; set; }                     // Foreign Key: vilken användare som skapade inlägget
    public User? User { get; set; }                     // Navigation property: referens till User-objektet

    // Koppling till Category (kategori)
    public int CategoryId { get; set; }                 // Foreign Key: vilken kategori inlägget tillhör
    public Category? Category { get; set; }             // Navigation property: referens till Category-objektet

    // Koppling till Comments (kommentarer)
    public List<Comment> Comments { get; set; } = new();
    // Navigation property: en post kan ha många kommentarer (1 post -> många comments)
}
