using System.ComponentModel.DataAnnotations;

namespace BlogCommunityApi.DTOs;

// DTO för att skapa ett nytt inlägg (POST /api/Posts)
// UserId hämtas från JWT (inloggad användare), därför skickas det inte i body.
public class CreatePostRequest
{
    [Required(ErrorMessage = "Title är obligatoriskt.")]
    [MaxLength(120, ErrorMessage = "Title får max vara 120 tecken.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Text är obligatoriskt.")]
    [MaxLength(5000, ErrorMessage = "Text får max vara 5000 tecken.")]
    public string Text { get; set; } = string.Empty;

    [Required(ErrorMessage = "CategoryId är obligatoriskt.")]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId måste vara större än 0.")]
    public int CategoryId { get; set; }
}

// DTO för att uppdatera ett befintligt inlägg (PUT /api/Posts/{id})
// Ägarkontroll görs via JWT (current user) + postens UserId i databasen.
public class UpdatePostRequest
{
    [Required(ErrorMessage = "Title är obligatoriskt.")]
    [MaxLength(120, ErrorMessage = "Title får max vara 120 tecken.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Text är obligatoriskt.")]
    [MaxLength(5000, ErrorMessage = "Text får max vara 5000 tecken.")]
    public string Text { get; set; } = string.Empty;

    [Required(ErrorMessage = "CategoryId är obligatoriskt.")]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId måste vara större än 0.")]
    public int CategoryId { get; set; }
}