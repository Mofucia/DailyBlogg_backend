using dailyblogg_backend.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace dailyblogg_backend.Repositories
{
    public interface IAuthRepository<T> where T : ApplicationUser
    {
        Task<T> CreateAsync(T entity, string password);
        Task<IList<string>> GetRoleAsync(T user);
        Task<IdentityResult> AddToRoleAsync(T user, string roleName);
        Task<bool> CheckPasswordAsync(T user, string password);
    }
}
