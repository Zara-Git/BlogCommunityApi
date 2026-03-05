using BlogCommunityApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogCommunityApi.Repositories
{
    // Repository-kontrakt för Posts: endast databasoperationer (EF Core)
    public interface IPostRepository
    {
        // ===== Read (public) =====

        // Hämtar alla inlägg (inkl. kategori + författare)
        Task<List<object>> GetAllAsync();

        // Sökning på titel (urval som matchar sökvillkoret)
        Task<List<object>> SearchByTitleAsync(string title);

        // Kontrollerar att kategori finns (för filter / by-category)
        Task<bool> CategoryExistsAsync(int categoryId);

        // Hämtar alla inlägg för en viss kategori
        Task<List<object>> GetByCategoryAsync(int categoryId);

       
        // Hämtar Post-entity (tracking) för update/delete + ägarkontroll
        Task<Post?> GetPostEntityByIdAsync(int id);

        // Kontrollerar att categoryId är giltig vid create/update
        Task<bool> CategoryValidAsync(int categoryId);

        // Skapar nytt inlägg och sparar
        Task<Post> AddAsync(Post post);

        // Sparar ändringar (t.ex. efter update)
        Task SaveChangesAsync();

        // Tar bort ett inlägg och sparar
        Task DeleteAsync(Post post);
    }
}