
using Application.Common.DTOs.AccountDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using MediatR;
using System.Security.Claims;

namespace Application.Features.Accounts.Commands.GenerateJwtToken
{
    public sealed class GenerateJwtTokenCommandHandler : IRequestHandler<GenerateJwtTokenCommand, Result<AuthResponseDto>>
    {
        private readonly IJwtProvider _jwtProvider;
        private readonly IAuthService _authService;

        public GenerateJwtTokenCommandHandler(IJwtProvider JwtProvider, IAuthService authService)
        {
            _jwtProvider = JwtProvider;
            _authService = authService;
        }

        public async Task<Result<AuthResponseDto>> Handle(GenerateJwtTokenCommand request, CancellationToken cancellationToken)
        {
            var claims = _jwtProvider.GetPrincipaleFromJwtToken(request.Token);

            if (claims is null)
                return Result.Failure<AuthResponseDto>(Error.Validation("", "Invalid Token"));

            var email = claims.FindFirst(ClaimTypes.Email)?.Value;

            var response = await _authService.GenerateNewJwtToken(email, request.RefreshToken);

            if (!response.IsAuthenticated)
                return Result.Failure<AuthResponseDto>(Error.Validation("", response.Message));

            return Result.Success(response);
            
        }
    }
}
