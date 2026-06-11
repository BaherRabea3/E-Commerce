using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Products.Commands.UpdateProduct
{
    public sealed record UpdateProductCommand(
            int id,
            string name,
            string description,
            decimal unitPrice,
            int quantity,
            IFormFile Image,
            int categoryId
        ) : IRequest<Result>
    {
    }
}
