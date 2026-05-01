using dailyblogg_backend.Data;
using dailyblogg_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace dailyblogg_backend.Repositories
{
    public class UserRepository<T> : IUserRepository<T> where T : ApplicationUser
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

        public async Task Remove(T entity) => _context.Set<T>().Remove(entity);

        public async Task<T?> GetUserByIdAsync(string id)
        {
            return await _context.Set<T>()
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<T?> FindByNameAsync(string userName)
        {
            return await _context.Set<T>()
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }
        public async Task<T?> FindByEmailAsync(string email)
        {
            return await _context.Set<T>()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<T>> GetAllUsersAsync()
        {
            return await _context.Set<T>()
                .OrderBy(u => u.UserName)
                .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetUsersByNameAsync(string name)
        {
            return await _context.Set<T>()
                .Where(u => u.Name != null && u.Name.Contains(name))
                .OrderBy(u => u.UserName)
                .ToListAsync();
        }

        public async Task<T> Update(T entity)
        {
            _context.Set<T>().Update(entity);
            await Task.CompletedTask;
            return entity;
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            if (user == null)
                return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }
    }
}
