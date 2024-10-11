using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QRMenuBackend.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            if (string.IsNullOrEmpty(jwtSettings["SecretKey"]))
{
    throw new InvalidOperationException("SecretKey is not configured.");
}
            var keyBytes = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? string.Empty);
            var secretKey = new SymmetricSecurityKey(keyBytes);
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
                signingCredentials: signinCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
