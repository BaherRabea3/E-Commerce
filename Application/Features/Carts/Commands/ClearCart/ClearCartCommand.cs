
using Domain.Common;
using MediatR;

namespace Application.Features.Carts.Commands.ClearCart
{
    public sealed record ClearCartCommand(int customerId) : IRequest<Result>
    {
    }
}
