
using Domain.Common;
using MediatR;

namespace Application.Features.Carts.Commands.DeleteCartItem
{
    public sealed record DeleteCartItemCommand(int id , int customerId) : IRequest<Result>
    {
    }
}
