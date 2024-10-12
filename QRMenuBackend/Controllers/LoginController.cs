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
public class LoginController : BaseController
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly TokenService _tokenService;

    public LoginController(UserManager<IdentityUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        var user = _userManager.FindByNameAsync(model.Username).Result;

        if (user != null && _userManager.CheckPasswordAsync(user, model.Password).Result)
        {
            var token = _tokenService.GenerateJwtToken(model.Username);
            return SuccessResponse(token, "Login successful");
        }

        return ErrorResponse("Invalid credentials.", new List<string> { "Kullanıcı adı veya şifre hatalı." }, 401);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (registerDto == null || string.IsNullOrEmpty(registerDto.Username) || string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.Email))
        {
            return ErrorResponse("Invalid registration details.");
        }

        var user = new IdentityUser
        {
            UserName = registerDto.Username,
            Email = registerDto.Email
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            return SuccessResponse("User registered successfully.");
        }

        return ErrorResponse(
            "User registration failed.",
            result.Errors?.Select(e => e.Description).ToList()
        );
    }

[FuPiCoAuthorize]
[HttpGet("data")]
public IActionResult GetProtectedData()
{
    return Ok(new { Message = "This is a protected data!" });
}
   }
   }
