using dailyblogg_backend.Models.Entities;

namespace dailyblogg_backend.Repositories
{
    public interface ICommentRepository<T> : IRepository<T> where T : Comment
    {
        Task<T?> GetCommentByUserId(string userId, int commentId);
    }
}
