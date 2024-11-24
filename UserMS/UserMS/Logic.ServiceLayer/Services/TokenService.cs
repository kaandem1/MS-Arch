using UserMS.Core.DomainLayer.Configuration;
using UserMS.Core.DomainLayer.Models;
using UserMS.Logic.ServiceLayer.IServices;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserMS.Logic.ServiceLayer.Services
{
    public class TokenService : ITokenService
    {
        private const int ExpirationMinutes = 90;

        private AuthenticationOptions _options;

        public TokenService(IOptions<AuthenticationOptions> options)
        {
            _options = options.Value;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescription = CreateJwtToken(
                CreateClaims(user),
                CreateSigningCredentials(),
                expiration
            );
            var token = tokenHandler.CreateToken(tokenDescription);

            return tokenHandler.WriteToken(token);
        }

        private SecurityTokenDescriptor CreateJwtToken(List<Claim> claims, SigningCredentials credentials,
            DateTime expiration) =>
               new SecurityTokenDescriptor
               {
                   Subject = new ClaimsIdentity(claims),
                   Expires = DateTime.UtcNow.AddMinutes(90),
                   Issuer = _options.ValidIssuer,
                   Audience = _options.ValidAudience,
                   SigningCredentials = credentials
               };


        private List<Claim> CreateClaims(User user)
        {
            try
            {
                var claims = new List<Claim>
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
            };

                return claims;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private SigningCredentials CreateSigningCredentials()
        {
            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_options.IssuerSecurityKey)
                ),
                SecurityAlgorithms.HmacSha512Signature
            );
        }

        private IEnumerable<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            return claims;
        }

    }
}
