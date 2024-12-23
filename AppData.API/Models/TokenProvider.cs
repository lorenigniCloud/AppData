using AppData.Infrastructures.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace AppData.API.Models
{
    public class TokenProvider
    {
        public IConfiguration _configuration { get; set; }

        public TokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> Create(IdentityUser model)
        {
            string secretKey = _configuration["JwtSettings:SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                new Claim(JwtRegisteredClaimNames.Sub, model.Id),
                        new Claim(JwtRegisteredClaimNames.Email, model.Email),
                new Claim("email_verified", model.EmailConfirmed.ToString())
                    }),
                Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpirationInMinutes")),
                SigningCredentials = credentials,
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            var handler = new JsonWebTokenHandler();
            string token = handler.CreateToken(tokenDescriptor);
            return token;
        }

    }
}
