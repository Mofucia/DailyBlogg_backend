using dailyblogg_backend.Models.Entities;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace dailyblogg_backend.Repositories
{
    public interface IPostRepository<T> : IRepository<T> where T : Post
    {
        Task<IEnumerable<T>> GetPostsByUserIdAsync(string userId);
        Task<T> GetPostByIdAsync(int postId);
        Task<IEnumerable<T>> GetAllPostsAsync();
        Task<IEnumerable<T>> GetAllPostsByTitleAsync(string title);
        Task<IEnumerable<T>> GetPostsByHashtagAsync(string hashtagName);
        Task<T> UpdateAsync(T entity);
    }
}
