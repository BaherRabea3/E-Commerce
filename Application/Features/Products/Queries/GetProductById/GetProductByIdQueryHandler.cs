using Application.Common.DTOs.ProductDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Queries.GetProductById
{
    public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IAppDbContext _context;

        public GetProductByIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                                  .AsNoTracking()
                                  .Where(p => p.Id == request.id)
                                  .Select(p => new ProductDto()
                                  {
                                      id          = p.Id,
                                      name        = p.Name,
                                      description = p.Description,
                                      quantity    = p.Quantity,
                                      unitPrice   = p.UnitPrice,
                                      imageUrl    = p.Image,
                                      category    = p.Category.Name
                                  })
                                  .FirstOrDefaultAsync(cancellationToken);

            if(product is null)
                return Result.Failure<ProductDto>(ProductErrors.NotFound(request.id));


            return Result<ProductDto>.Success(product);
        }
    }
}
