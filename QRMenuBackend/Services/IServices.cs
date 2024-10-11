namespace QRMenuBackend.Services
{
    public interface IService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id); // ID tipi string olarak güncellendi
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id); // ID tipi string olarak güncellendi
    }
}
