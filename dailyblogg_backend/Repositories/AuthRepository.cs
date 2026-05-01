using dailyblogg_backend.Data;
using dailyblogg_backend.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace dailyblogg_backend.Repositories
{
    public class AuthRepository<T> : IAuthRepository<T> where T : ApplicationUser
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<T> _userManager;

        public AuthRepository(ApplicationDbContext context, UserManager<T> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<T> CreateAsync(T entity,string password)
        {
            var result = await _userManager.CreateAsync(entity,password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException();
            }
            return entity;
        }

        public async Task<IList<string>> GetRoleAsync(T user)
        {
            if (user == null)
                return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        public async Task<IdentityResult> AddToRoleAsync(T user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<bool> CheckPasswordAsync(T user, string password)
        {
            if (user == null)
                return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}
