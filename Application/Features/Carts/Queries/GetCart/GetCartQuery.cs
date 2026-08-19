
using Application.Common.DTOs.CartDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Carts.Queries.GetCart
{
    public sealed record GetCartQuery(int customerId) : IRequest<Result<CartDetailsDto>>
    {
    }
}
