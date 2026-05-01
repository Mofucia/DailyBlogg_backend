using dailyblogg_backend.Data;
using dailyblogg_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace dailyblogg_backend.Repositories
{
    public class FriendshipRepository<T> : IFriendshipRepository<T> where T : Friendship
    {
        private readonly ApplicationDbContext _context;
        public FriendshipRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
        public async Task Remove(T entity) => _context.Set<T>().Remove(entity);

        public async Task<T?> FindRequest(string sender, string receiver)
        {
            return await _context.Set<T>()
                                 .Include(f => f.Requestor)
                                 .Include(f => f.Receiver)
                                 .FirstOrDefaultAsync(f => f.RequestorId == sender && f.ReceiverId == receiver);
        }

        public async Task<IEnumerable<T>> GetAllFriendship(string userId)
        {
            return await _context.Set<T>()
                .Where(f => (f.RequestorId == userId || f.ReceiverId == userId) && f.Status == FriendshipStatus.Accepted)
                .Include(f => f.Requestor)
                .Include(f => f.Receiver)
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllPendingRequest(string userId)
        {
            // pending requests received by the user
            return await _context.Set<T>()
                .Where(f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending)
                .Include(f => f.Requestor)
                .ToListAsync();
        }

    }
}
