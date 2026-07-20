
using Application.Common.DTOs.AccountDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Accounts.Commands.Register
{
    public sealed record RegisterCommand
        (string FirstName,
        string LastName,
        string Email,
        string Password,
        string ConfirmPassword) : IRequest<Result<AuthResponseDto>>
    {
    }
}
