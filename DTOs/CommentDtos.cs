using System.ComponentModel.DataAnnotations;

namespace BlogCommunityApi.DTOs;

// DTO för att skapa en kommentar (POST /api/Comments)
// UserId hämtas från JWT (inloggad användare), därför skickas det inte i body.
public class CreateCommentRequest
{
    [Required]
    public int PostId { get; set; }

    [Required]
    public string Text { get; set; } = "";
}