using Application.Common.DTOs.AccountDTOs;
using Application.Common.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtProvider _jwtProvider;
        public AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
        }

        public async Task<AuthResponseDto> GenerateNewJwtToken(string Email, string RefreshToken)
        {
            var appUser = await _userManager.FindByEmailAsync(Email);

            if (appUser is null ||
                appUser.RefreshToken != RefreshToken ||
                appUser.RefreshTokenExpiration < DateTime.UtcNow)
                return new AuthResponseDto { Message = " Invalid Refresh Token" };

            var response = await _jwtProvider.GenerateTokenAsync(Email);

            response.IsAuthenticated = true;

            appUser.RefreshToken = response.RefreshToken;
            appUser.RefreshTokenExpiration = response.RefreshTokenExpiration;

            await _userManager.UpdateAsync(appUser);

            return response;
        }

        public async Task<AuthResponseDto> LoginAsync(string Email, string Password)
        {
           var user = await _userManager.FindByEmailAsync(Email);

            if(user is null)
                return new AuthResponseDto { Message = "email or password are not correct"};

           bool isValid = await _userManager.CheckPasswordAsync(user, Password);

            if (!isValid)
                return new AuthResponseDto { Message = "email or password are not correct" };


            // generate token and refresh token

            var authResponse = await _jwtProvider.GenerateTokenAsync(user.Email!);

            authResponse.IsAuthenticated = true;

            user.RefreshToken = authResponse.RefreshToken;
            user.RefreshTokenExpiration = authResponse.RefreshTokenExpiration;

            await _userManager.UpdateAsync(user);

            return authResponse;
        }

        public async Task LogoutAsync(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            user!.RefreshToken = null;

            user.RefreshTokenExpiration = null;

            await _userManager.UpdateAsync(user);
        }

        public async Task<AuthResponseDto> RegisterAsync(string FirstName, string LastName, string Email, string Password)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if (user is not null)
                return new AuthResponseDto { Message = "User already register" };

            var appUser = new ApplicationUser
            {
                UserName = Email.Split('@')[0],
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
            };

            var result = await _userManager.CreateAsync(appUser, Password);

            if (!result.Succeeded)
            {
                var errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));

                return new AuthResponseDto { Message = errorMessage };
            }

            var RoleResult = await _userManager.AddToRoleAsync(appUser, "Customer");

            if (!RoleResult.Succeeded)
            {
                var errorMessage = string.Join(" | ", RoleResult.Errors.Select(e => e.Description));

                return new AuthResponseDto { Message = errorMessage };
            }

            // generate token and refresh token

            var authResponse = await _jwtProvider.GenerateTokenAsync(Email);

            authResponse.IsAuthenticated = true;

            appUser.RefreshToken = authResponse.RefreshToken;
            appUser.RefreshTokenExpiration = authResponse.RefreshTokenExpiration;

            await _userManager.UpdateAsync(appUser);

            return authResponse;
        }
    }
}
