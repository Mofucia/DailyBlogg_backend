using dailyblogg_backend.Models.Entities;

namespace dailyblogg_backend.Repositories
{
    public interface IStoryRepository<T> : IRepository<T> where T : Story
    {
        Task<T?> GetStoryById(int storyId);
        Task<List<T>> AllActiveStory(DateTime storyExpireDate);
    }
}
