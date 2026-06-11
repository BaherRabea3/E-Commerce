using Domain.Common;
using MediatR;

namespace Application.Features.Products.Commands.DeleteProduct
{
    public sealed record DeleteProductCommand(int id) : IRequest<Result>
    {
    }
}
