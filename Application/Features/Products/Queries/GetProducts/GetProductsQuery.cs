using Application.Common.DTOs;
using Application.Common.DTOs.ProductDTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.Products.Queries.GetProducts
{
    public sealed record GetProductsQuery(int? categoryId , int? price , string? search , int? page , int? pageSize) 
        : IRequest<Result<PaginatedResult<ProductDto>>>
    {
    }
}
