using dailyblogg_backend.Data;
using dailyblogg_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace dailyblogg_backend.Repositories
{
    public class LikeRepository<T> : ILikeRepository<T> where T : Like
    {
        private readonly ApplicationDbContext _context;
        public LikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public async Task Remove(T entity) => _context.Set<T>().Remove(entity);

        public async Task<int> LikeCountForPost(int postId)
        {
            return await _context.Likes.CountAsync(l => l.PostId == postId);
        }

        public async Task<bool> HasLikedByCurrentUser(int postId, string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            return await _context.Likes.AnyAsync(l => l.PostId == postId && l.UserId == userId);
        }
        public async Task<T?> PostLike(string userId, int postId)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
        }
    }
}
