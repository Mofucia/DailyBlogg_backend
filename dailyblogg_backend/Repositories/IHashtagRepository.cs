using dailyblogg_backend.Models.Entities;

namespace dailyblogg_backend.Repositories
{
    public interface IHashtagRepository<T> : IRepository<T> where T : Hashtag
    {
        Task<List<T>> GetAllAsync();
        Task<bool> HashtagExistsAsync(string name);
        Task<List<string>> GetTrendingAsync();
        Task<T?> GetByNameAsync(string name);
    }
}
