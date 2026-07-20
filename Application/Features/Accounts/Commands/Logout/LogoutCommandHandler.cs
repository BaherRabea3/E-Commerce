
using Application.Common.Interfaces;
using Domain.Common;
using MediatR;

namespace Application.Features.Accounts.Commands.Logout
{
    public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IAuthService _authService;

        public LogoutCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _authService.LogoutAsync(request.Email);

            return Result.Success();
        }
    }
}
