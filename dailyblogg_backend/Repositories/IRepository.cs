namespace dailyblogg_backend.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task SaveChangesAsync();
        Task AddAsync(T entity);
        Task Remove(T entity);
    }
}
