using dailyblogg_backend.Models.DTOs;

namespace dailyblogg_backend.Repositories
{
    public interface IFriendshipRepository<T> :  IRepository<T> where T : class
    {
        Task<T?> FindRequest(string sender, string receiver);
        Task<IEnumerable<T>> GetAllFriendship(string userId);

        Task<IEnumerable<T>> GetAllPendingRequest(string userId);

    }
}
