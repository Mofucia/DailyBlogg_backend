using dailyblogg_backend.Data;
using dailyblogg_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace dailyblogg_backend.Repositories
{
    public class HashtagRepository<T> : IHashtagRepository<T> where T : Hashtag
    {
        private readonly ApplicationDbContext _context;
        public HashtagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public async Task Remove(T entity) => _context.Set<T>().Remove(entity);

        public async Task<bool> HashtagExistsAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return await _context.Set<T>().AnyAsync(h => h.HashtagName == name);
        }

        public async Task<List<string>> GetTrendingAsync()
        {
            // Trending by number of associated posts (descending)
            return await _context.Set<T>()
                                 .Include(h => h.PostHashtags)
                                 .OrderByDescending(h => h.PostHashtags.Count)
                                 .Take(10)
                                 .Select(h => h.HashtagName)
                                 .ToListAsync();
        }

        public async Task<T?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return await _context.Set<T>().FirstOrDefaultAsync(h => h.HashtagName == name);
        }
    }
}
