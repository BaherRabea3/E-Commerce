using Domain.Common;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProductStock
{
    public sealed record UpdateProductStockCommand(int id , int quantity) : IRequest<Result>
    {
    }
}
