
using Application.Common.DTOs.AccountDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Accounts.Commands.Register
{
    public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        public RegisterCommandHandler(IAppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var response = await _authService.RegisterAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password
                );

            if (!response.IsAuthenticated)
            {
                return Result.Failure<AuthResponseDto>(Error.Validation("Account.ValidationError", response.Message));
            }

            return Result.Success(response);
        }
    }
}
