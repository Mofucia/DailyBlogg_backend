using dailyblogg_backend.Models.Entities;

namespace dailyblogg_backend.Repositories
{
    public interface ILikeRepository<T> : IRepository<T> where T : Like
    {
        Task<int> LikeCountForPost(int postId);
        Task<bool> HasLikedByCurrentUser(int postId, string userId);
        Task<T?> PostLike(string userId, int postId);
    }
}
