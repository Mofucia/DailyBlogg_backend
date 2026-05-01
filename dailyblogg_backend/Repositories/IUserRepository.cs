using dailyblogg_backend.Models.Entities;

namespace dailyblogg_backend.Repositories
{
    public interface IUserRepository<T> : IRepository<T> where T : ApplicationUser
    {
        Task<T?> GetUserByIdAsync(string id);
        Task<T?> FindByNameAsync(string userName);
        Task<T?> FindByEmailAsync(string email);
        Task<IEnumerable<T>> GetAllUsersAsync();
        Task<IEnumerable<T>> GetUsersByNameAsync(string name);
        Task<T> Update(T entity);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        
    }
}
