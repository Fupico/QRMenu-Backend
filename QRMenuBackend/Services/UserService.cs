using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QRMenuBackend.Data;
using QRMenuBackend.Repositories;
using QRMenuBackend.Services;

namespace QRMenuBackend.Services
{
    public class UserService : IService<IdentityUser>
    {
        private readonly IRepository<IdentityUser> _repository;

        public UserService(IRepository<IdentityUser> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<IdentityUser>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IdentityUser> GetByIdAsync(string id) // id'yi string olarak değiştirdik
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(IdentityUser entity)
        {
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(IdentityUser entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(string id) // id'yi string olarak değiştirdik
        {
            await _repository.DeleteAsync(id);
        }
    }
}
