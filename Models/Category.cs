using System.ComponentModel.DataAnnotations;   // DataAnnotations – validering som [Required]

namespace BlogCommunityApi.Models;

// Entity/Model för tabellen "Categories"
// Den här klassen beskriver hur en kategori ser ut i databasen.
public class Category
{
    public int Id { get; set; }                // Primärnyckel (auto-increment i SQL Server via EF)

    [Required]                                 // Name är obligatoriskt (får inte vara null/tomt i DB)
    public string Name { get; set; } = "";     // Kategorins namn (t.ex. "Training", "Fashion", "Health")

    // Navigation property:
    // En kategori kan ha många posts (1 kategori -> många inlägg)
    public List<Post> Posts { get; set; } = new(); // Lista med inlägg som tillhör denna kategori
}
