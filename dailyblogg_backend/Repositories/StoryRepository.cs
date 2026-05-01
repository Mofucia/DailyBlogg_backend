using dailyblogg_backend.Data;
using dailyblogg_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace dailyblogg_backend.Repositories
{
    public class StoryRepository<T> : IStoryRepository<T> where T : Story
    {
        private readonly ApplicationDbContext _context;
        public StoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public async Task Remove(T entity) => _context.Set<T>().Remove(entity);
        public async Task<T?> GetStoryById(int storyId)
        {
            return await _context.Set<T>().Include(s => s.User).FirstOrDefaultAsync(s => s.Id == storyId);
        }
        public async Task<List<T>> AllActiveStory(DateTime storyExpireDate)
        {
            return await _context.Set<T>()
                                 .Include(p => p.User)
                                 .Where(p => p.CreatedDate >= storyExpireDate) // Only get story that is not expire
                                 .OrderByDescending(p => p.CreatedDate)
                                 .ToListAsync();
        }
    }
}
