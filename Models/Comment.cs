namespace BlogCommunityApi.Models;

// Entity/Model för tabellen "Comments"
// Den här klassen beskriver hur en kommentar ser ut i databasen och hur den kopplas till Post och User.
public class Comment
{
    public int Id { get; set; }                        // Primärnyckel (auto-increment i databasen)

    public string Text { get; set; } = "";             // Själva kommentaren (texten som användaren skriver)

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // När kommentaren skapades (UTC). Default sätts direkt när objektet skapas.

    public int PostId { get; set; }                    // Foreign Key: vilket inlägg kommentaren tillhör
    public Post? Post { get; set; }                    // Navigation property: referens till Post-objektet

    public int UserId { get; set; }                    // Foreign Key: vilken användare som skrev kommentaren
    public User? User { get; set; }                    // Navigation property: referens till User-objektet
}
