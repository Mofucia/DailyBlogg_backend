using dailyblogg_backend.Data;
using dailyblogg_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace dailyblogg_backend.Repositories
{
    public class NotificationRepository<T> : INotificationRepository<T> where T : Notification
    {
        private readonly ApplicationDbContext _context;
        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public async Task Remove(T entity) => _context.Set<T>().Remove(entity);
        public async Task<T?> FindNotification(int notificationId)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(n => n.Id == notificationId);
        }
        public async Task<IEnumerable<T>> GetAllNotificationsByUserId(string userId)
        {
            return await _context.Set<T>().Where(n => n.UserId == userId)
                                          .OrderByDescending(n => n.CreatedAt)
                                          .ToListAsync();
        }
    }
}
