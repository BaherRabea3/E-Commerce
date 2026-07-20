
using Application.Common.DTOs.AccountDTOs;
using Application.Common.Interfaces;
using Application.Common.Settings;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services.AuthServices
{
    public class JwtProvider : IJwtProvider
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<JwtOptions> _options;

        public JwtProvider(IOptions<JwtOptions> options, UserManager<ApplicationUser> userManager)
        {
            _options = options;
            _userManager = userManager;
        }

        public async Task<AuthResponseDto> GenerateTokenAsync(string email)
        {

            var appUser = await _userManager.FindByEmailAsync(email);


            var claims = new List<Claim>
            {
                 new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,appUser!.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
            };

            var roles = await _userManager.GetRolesAsync(appUser);

            foreach (var role in roles) 
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow
                .AddMinutes(Convert.ToDouble(_options.Value.Expiration_Minutes));

            JwtSecurityToken tokenGenerator = new JwtSecurityToken
                (
                    issuer: _options.Value.Issuer,
                    audience: _options.Value.Audience,
                    claims: claims,
                    signingCredentials: credentials,
                    expires: expiration
                );

            JwtSecurityTokenHandler JwtHandler = new JwtSecurityTokenHandler();
            string token = JwtHandler.WriteToken(tokenGenerator);

            return new AuthResponseDto()
            {
                UserId = appUser.Id,
                Email = email,
                Token = token,
                TokenExpiration = expiration,
                IsAuthenticated = true,
                RefreshToken = CreateRefreshToken(),
                RefreshTokenExpiration = DateTime.UtcNow
                .AddDays(Convert.ToDouble(_options.Value.RefreshToken_Expiration_Days))
                
            };
        }

        public ClaimsPrincipal? GetPrincipaleFromJwtToken(string? jwtToken)
        {
            var TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = _options.Value.Issuer,
                ValidateAudience = true,
                ValidAudience = _options.Value.Audience,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey))

            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var Claims = handler.ValidateToken(jwtToken, TokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }

            return Claims;

        }

        private string CreateRefreshToken()
        {
            byte[] bytes = new byte[64];
            var randomGenerator = RandomNumberGenerator.Create();
            randomGenerator.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }
    }
}
