using Application.Common.DTOs.AccountDTOs;
using System.Security.Claims;

namespace Application.Common.Interfaces
{
    public interface IJwtProvider
    {
        Task<AuthResponseDto> GenerateTokenAsync(string email);

        ClaimsPrincipal? GetPrincipaleFromJwtToken(string? jwtToken);
    }
}
