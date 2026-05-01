using dailyblogg_backend.Data;
using dailyblogg_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace dailyblogg_backend.Repositories
{
    public class CommentRepository<T> : ICommentRepository<T> where T : Comment
    {
        private readonly ApplicationDbContext _context;
        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public async Task Remove(T entity) => _context.Set<T>().Remove(entity);
        public async Task<T?> GetCommentByUserId(string userId, int commentId) => await _context.Set<T>().FirstOrDefaultAsync(c => c.UserId == userId && c.Id == commentId);
    }
}
