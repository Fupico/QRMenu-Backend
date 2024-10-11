namespace QRMenuBackend.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);  // id'yi string olarak değiştirdik
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);  // id'yi string olarak değiştirdik
    }
}
