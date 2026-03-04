using BlogCommunityApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogCommunityApi.Repositories
{
    public interface ICommentRepository
    {
        Task<Post?> GetPostByIdAsync(int postId);         
        Task<Comment> AddAsync(Comment comment);          
        Task<List<object>> GetByPostAsync(int postId);    
    }
}