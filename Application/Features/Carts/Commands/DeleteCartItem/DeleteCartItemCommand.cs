
using Domain.Common;
using MediatR;

namespace Application.Features.Carts.Commands.DeleteCartItem
{
    public sealed record DeleteCartItemCommand(int id , Guid customerId) : IRequest<Result>
    {
    }
}
