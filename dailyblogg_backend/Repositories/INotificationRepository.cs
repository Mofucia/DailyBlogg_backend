namespace dailyblogg_backend.Repositories
{
    public interface INotificationRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllNotificationsByUserId(string userId);
        Task<T?> FindNotification(int notificationId);
    }
}
