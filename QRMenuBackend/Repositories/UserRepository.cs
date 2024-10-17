using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QRMenuBackend.Data;

namespace QRMenuBackend.Repositories
{
    public class UserRepository : IRepository<IdentityUser>
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IdentityUser>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

public async Task<IdentityUser> GetByIdAsync(string id)
{
    var user = await _context.Users.FindAsync(id);
    if (user == null)
    {
        throw new Exception("User not found");
    }
    return user;
}


        public async Task AddAsync(IdentityUser entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(IdentityUser entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id) // id'yi string yaptık
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
