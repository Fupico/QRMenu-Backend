using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using QRMenuBackend.Models;
using System.Linq;
using QRMenuBackend.Services;
using System.Security.Claims;
using QRMenuBackend.Data;
using QRMenuBackend.Entities;

namespace QRMenuBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenService _tokenService;

        private readonly AppDbContext _context;
        public LoginController(UserManager<IdentityUser> userManager, TokenService tokenService, AppDbContext context)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                return ErrorResponse("Invalid login details."); // 400 durum kodu varsayılan
            }

            var user = _userManager.FindByNameAsync(model.Username).Result;

            if (user == null || !_userManager.CheckPasswordAsync(user, model.Password).Result)
            {
                return ErrorResponse("Invalid credentials.", new List<string> { "Kullanıcı adı veya şifre hatalı." }, 400);
            }

            if (!user.EmailConfirmed)
            {
                return ErrorResponse(
                    "Account not verified.",
                    new List<string> { "Yönetici ile görüşün hesabınız onaylı değildir. (İletişim : 0543 819 4976)" },
                    400
                );
            }

            var token = _tokenService.GenerateJwtToken(model.Username, user.Id);
            return SuccessResponse(token, "Login successful"); // 200 durum kodu varsayılan
        }



        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null || string.IsNullOrEmpty(registerDto.Username) || string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.Email))
            {
                return ErrorResponse("Invalid registration details.", new List<string> { "Tüm alanlar doldurulmalıdır." });
            }

            var user = new IdentityUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errorMessages = result.Errors?.Select(e => e.Description).ToList() ?? new List<string>();
                return ErrorResponse("User registration failed.", errorMessages);
            }

            // Kullanıcı başarıyla oluşturuldu, şimdi şirket kaydı oluşturuyoruz.
            var newCompany = new CompanyEntity
            {
                Name = registerDto.Username, // DTO'nuzda şirket adı gibi gerekli alanları eklemelisiniz
                Domain = $"https://qrmenu.fupico.com/menu/{user.Id}", // Domain userId ile oluşturuluyor
                ImageUrl = null, // Kullanıcı şirket logosu seçmiş olabilir
                CreatedBy = user.Id,
                CreatedAt = DateTime.Now
            };

            _context.Companies.Add(newCompany);
            await _context.SaveChangesAsync();

            return SuccessResponse("User and company registered successfully.");
        }

    }
}
