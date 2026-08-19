using Domain.Common;
using MediatR;

namespace Application.Features.Carts.Commands.AddToCart
{
    public sealed record AddToCartCommand(int productId , int quantity , int customerId) : IRequest<Result<int>>
    {
    }
}
