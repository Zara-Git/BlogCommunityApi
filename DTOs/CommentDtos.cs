using System.ComponentModel.DataAnnotations;

namespace BlogCommunityApi.DTOs;

// DTO för att skapa en kommentar (POST /api/Comments)
// UserId hämtas från JWT (inloggad användare), därför skickas det inte i body.
public class CreateCommentRequest
{
    // Vilket inlägg kommentaren tillhör
    [Required(ErrorMessage = "PostId är obligatoriskt.")]
    [Range(1, int.MaxValue, ErrorMessage = "PostId måste vara större än 0.")]
    public int PostId { get; set; }

    // Själva kommentaren
    [Required(ErrorMessage = "Text är obligatoriskt.")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Text måste vara mellan 1 och 500 tecken.")]
    public string Text { get; set; } = string.Empty;
}