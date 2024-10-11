using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QRMenuBackend.Services
{
    public class UserService : IService<IdentityUser>
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

public async Task<IdentityUser> GetByIdAsync(string id)
{
    var user = await _userManager.Users.FirstOrDefaultAsync(user => user.Id == id);
    if (user == null)
    {
        throw new Exception("User not found"); // Null ise exception fırlatabilirsiniz
    }
    return user;
}

    public async Task<IEnumerable<IdentityUser>> GetAllAsync()
    {
        return await _userManager.Users.ToListAsync();
    }

        public async Task AddAsync(IdentityUser user)
        {
            await _userManager.CreateAsync(user);
        }

        public async Task UpdateAsync(IdentityUser user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }
    }
}
