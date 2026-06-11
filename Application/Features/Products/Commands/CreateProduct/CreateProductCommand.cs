using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Products.Commands.CreateProduct
{
    public sealed record CreateProductCommand(
            string name ,
            string description ,
            decimal unitPrice ,
            int quantity ,
            IFormFile Image ,
            int categoryId
        ) : IRequest<Result<int>>
    {
    }
}
