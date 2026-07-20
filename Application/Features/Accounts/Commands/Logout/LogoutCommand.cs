
using Domain.Common;
using MediatR;

namespace Application.Features.Accounts.Commands.Logout
{
    public sealed record LogoutCommand(string Email) : IRequest<Result>
    {
    }
}
