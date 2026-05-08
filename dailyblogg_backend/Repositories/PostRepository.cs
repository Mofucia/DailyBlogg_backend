using dailyblogg_backend.Data;
using dailyblogg_backend.Models.DTOs;
using dailyblogg_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace dailyblogg_backend.Repositories
{
    public class PostRepository<T> : IPostRepository<T> where T : Post
    {
        private readonly ApplicationDbContext _context;
        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public async Task Remove(T entity) => _context.Set<T>().Remove(entity);
        public async Task<IEnumerable<T>> GetPostsByUserIdAsync(string userId)
        {
            try
            {
                return await _context.Set<T>()
                    .Where(p => p.UserId == userId)
                    .Include(p => p.User)
                    .Include(p => p.Comments).ThenInclude(c => c.User)
                    .Include(p => p.Likes).ThenInclude(l => l.User)
                    .Include(p => p.Hashtags)
                    .OrderByDescending(p => p.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving posts for user {userId}: {ex.Message}", ex);
            }
        }
        public async Task<T?> GetPostByIdAsync(int postId)
        {
            return await _context.Set<T>()
                .Include(p => p.User)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Likes).ThenInclude(l => l.User)
                .Include(p => p.Hashtags)
                .FirstOrDefaultAsync(p => p.Id == postId);
        }
        public async Task<IEnumerable<T>> GetAllPostsAsync()
        {
            return await _context.Set<T>()
                .Include(p => p.User)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Likes).ThenInclude(l => l.User)
                .Include(p => p.Hashtags)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllPostsByTitleAsync(string title)
        {
            return await _context.Set<T>()
                .Where(p => p.Title != null && p.Title.Contains(title))
                .Include(p => p.User)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Likes).ThenInclude(l => l.User)
                .Include(p => p.Hashtags)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPostsByHashtagAsync(string hashtagName)
        {
            if (string.IsNullOrWhiteSpace(hashtagName))
                return Enumerable.Empty<T>();

            var name = hashtagName.Trim().ToLower();

            return await _context.Set<T>()
                .Where(p => p.Hashtags.Any(h => h.HashtagName.ToLower() == name))
                .Include(p => p.User)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Likes).ThenInclude(l => l.User)
                .Include(p => p.Hashtags)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await Task.CompletedTask;
            return entity;
        }
    }
}
