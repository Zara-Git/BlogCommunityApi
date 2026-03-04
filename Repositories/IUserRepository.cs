using BlogCommunityApi.Models;
using System.Threading.Tasks;

namespace BlogCommunityApi.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<User> AddAsync(User user);
    }
}