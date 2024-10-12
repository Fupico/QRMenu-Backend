using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using QRMenuBackend.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace QRMenuBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IService<IdentityUser> _userService;

        public UserController(IService<IdentityUser> userService)
        {
            _userService = userService;
        }

[HttpGet("allusers")] // Burada özel bir route tanımlıyoruz
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

[HttpPost("register")] // Burada özel bir route tanımlıyoruz
        public async Task<IActionResult> AddUser([FromBody] IdentityUser user)
        {
            await _userService.AddAsync(user);
            return Ok();
        }
    }
}
