using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

public class FuPiCoSecurityAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var configuration = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
        var secretKey = configuration["JwtSettings:SecretKey"]; // appsettings.json'dan key alınıyor

        if (string.IsNullOrEmpty(secretKey))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = context.HttpContext.Request.Headers["FuPiCo-Security"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secretKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "FuPiCoMenu",
                ValidAudience = "FuPiCoAudience",
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero // Expiry süresinde tolerans yok
            }, out SecurityToken validatedToken);

            // Token içerisindeki userId claim'ini al
            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // userId'yi HttpContext'e kaydet
            context.HttpContext.Items["userId"] = userId;
        }
        catch
        {
            // Token geçersizse yetkisiz dönüş yapılır
            context.Result = new UnauthorizedResult();
            return;
        }
    }
}
