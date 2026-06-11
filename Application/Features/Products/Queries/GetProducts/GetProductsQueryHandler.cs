using Application.Common.DTOs;
using Application.Common.DTOs.ProductDTOs;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Queries.GetProducts
{
    public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PaginatedResult<ProductDto>>>
    {
        private readonly IAppDbContext _context;

        public GetProductsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResult<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.search))
                query = _context.Products
                    .ApplyFilter(p => p.Name.ToLowerInvariant().Contains(request.search.ToLowerInvariant()));

            if (request.price.HasValue)
                query = query
                    .ApplyFilter(p => p.UnitPrice == request.price.Value);

            if (request.categoryId.HasValue)
                query = query
                    .ApplyFilter(p => p.CategoryId == request.categoryId.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var result = await query.ApplyPagination(request.page, request.pageSize)
                         .ToListAsync();

            var response = new PaginatedResult<ProductDto>()
                    {
                        page = request.page ?? 1,
                        pageSize = request.pageSize ?? 8,
                        totalCount = totalCount,
                        items = result.Select(p => new ProductDto()
                        {
                            id = p.Id,
                            name = p.Name,
                            description = p.Description,
                            quantity = p.Quantity,
                            unitPrice = p.UnitPrice,
                            imageUrl = p.Image,
                            category = p.Category.Name
                        })
                    };

            return Result<PaginatedResult<ProductDto>>.Success(response);
        }
    }
}
