
using Domain.Common;
using MediatR;

namespace Application.Features.Carts.Commands.UpdateCartItem
{
    public sealed record UpdateCartItemCommand(int cartItemId, int quantity, Guid customerId) : IRequest<Result>
    {
    }
}
