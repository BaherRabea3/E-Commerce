
using Application.Common.DTOs.AccountDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Accounts.Commands.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var response = await _authService.LoginAsync(request.Email, request.Password);

            if (!response.IsAuthenticated)
                return Result.Failure<AuthResponseDto>(Error.Validation("Account.ValidationError", response.Message));

            return Result.Success(response);
        }
    }
}
