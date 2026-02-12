using System.ComponentModel.DataAnnotations;

namespace BlogCommunityApi.DTOs;

// DTO för att skapa ett nytt inlägg (POST /api/Posts)
// UserId hämtas från JWT (inloggad användare), därför skickas det inte i body.
public class CreatePostRequest
{
    [Required, MaxLength(120)]
    public string Title { get; set; } = "";

    [Required]
    public string Text { get; set; } = "";

    [Required]
    public int CategoryId { get; set; }
}

// DTO för att uppdatera ett befintligt inlägg (PUT /api/Posts/{id})
// Ägarkontroll görs via JWT (current user) + postens UserId i databasen.
public class UpdatePostRequest
{
    [Required, MaxLength(120)]
    public string Title { get; set; } = "";

    [Required]
    public string Text { get; set; } = "";

    [Required]
    public int CategoryId { get; set; }
}