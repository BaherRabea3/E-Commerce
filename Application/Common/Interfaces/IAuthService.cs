
using Application.Common.DTOs.AccountDTOs;

namespace Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(
            string FirstName,
            string LastName,
            string Email,
            string Password);

        Task<AuthResponseDto> LoginAsync(
           string Email,
           string Password);

        Task LogoutAsync(string Email);

        Task<AuthResponseDto> GenerateNewJwtToken(string Email, string RefreshToken);
    }
}
