
using Application.Common.DTOs.AccountDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Accounts.Commands.GenerateJwtToken
{
    public sealed record GenerateJwtTokenCommand(string Token, string RefreshToken) : IRequest<Result<AuthResponseDto>>
    {
    }
}
