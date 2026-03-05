using BlogCommunityApi.Models;

namespace BlogCommunityApi.Repositories
{
    // Repository-kontrakt för User: endast DB-operationer (EF Core)
    public interface IUserRepository
    {
        // Auth (Register/Login) 

        // Kontrollerar om användarnamn redan finns
        Task<bool> UsernameExistsAsync(string username);

        // Kontrollerar om email redan finns
        Task<bool> EmailExistsAsync(string email);

        // Hämtar användare via username (för login)
        Task<User?> GetByUsernameAsync(string username);

        // Lägger till en ny användare och sparar i databasen
        Task<User> AddAsync(User user);

        //  Update (Users/me)

        // Hämtar användare via id (utan Include)
        Task<User?> GetByIdAsync(int id);

        // Kontrollerar om ett nytt username är upptaget (exkluderar aktuell användare)
        Task<bool> UsernameTakenAsync(string username, int excludeUserId);

        // Kontrollerar om ett nytt email är upptaget (exkluderar aktuell användare)
        Task<bool> EmailTakenAsync(string email, int excludeUserId);

        // Sparar ändringar (t.ex. efter update av username/email/password)
        Task SaveChangesAsync();

        // Delete

        // Hämtar användaren med relaterad data (Posts/Comments/RefreshTokens) för delete
        Task<User?> GetUserWithRelationsAsync(int id);

        // Tar bort användaren och relaterad data (för att undvika FK-problem)
        Task DeleteUserWithRelationsAsync(User user);
    }
}