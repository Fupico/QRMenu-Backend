using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using QRMenuBackend.Models;
using System.Linq; // Ekleyin
using QRMenuBackend.Services; // TokenService'in bulunduğu namespace
using System.Security.Claims; // Ekleyin

namespace QRMenuBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenService _tokenService; // TokenService için referans

        public LoginController(UserManager<IdentityUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService; // TokenService'i başlat
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _userManager.FindByNameAsync(model.Username).Result;

            if (user != null && _userManager.CheckPasswordAsync(user, model.Password).Result)
            {
                var token = _tokenService.GenerateJwtToken(model.Username);
                return Ok(new ApiResponse<string>(token));
            }

            return Unauthorized(new ApiResponse<string>("Invalid credentials."));
        }

 [AllowAnonymous]
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
{
    if (registerDto == null || string.IsNullOrEmpty(registerDto.Username) || string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.Email))
    {
        return BadRequest(new ApiResponse<string>("Invalid registration details."));
    }

    var user = new IdentityUser
    {
        UserName = registerDto.Username,
        Email = registerDto.Email
    };

    var result = await _userManager.CreateAsync(user, registerDto.Password);

    if (result.Succeeded)
    {
        return Ok(new ApiResponse<string>("User registered successfully."));
    }

    // Hataları kontrol et
    if (result.Errors != null && result.Errors.Any())
    {
        return BadRequest(new ApiResponse<string>(
            "User registration failed: " + 
            string.Join(", ", result.Errors.Select(e => e.Description))
        ));
    }

    // Eğer hata yoksa, genel bir hata mesajı döndür
    return BadRequest(new ApiResponse<string>("User registration failed."));
}
      [Authorize]
        [HttpGet("protected")]
        public IActionResult GetProtectedResource()
        {
            return Ok("This is a protected resource.");
        }
    
}}
