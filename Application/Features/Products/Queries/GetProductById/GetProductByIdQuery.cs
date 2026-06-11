using Application.Common.DTOs.ProductDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Products.Queries.GetProductById
{
    public sealed record GetProductByIdQuery(int id) : IRequest<Result<ProductDto>>
    {
    }
}
