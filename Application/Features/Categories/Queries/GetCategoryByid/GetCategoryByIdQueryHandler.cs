using Application.Common.DTOs.CategoryDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Categories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Queries.GetCategoryByid
{
    public sealed class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDetailsDto>>
    {
        private readonly IAppDbContext _context;

        public GetCategoryByIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CategoryDetailsDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                                    .AsNoTracking()
                                    .Select(c => new CategoryDetailsDto
                                    {
                                        id = c.Id,
                                        name = c.Name,
                                        description = c.Description,
                                        productsCount = c.Products.Count()
                                    })
                                    .FirstOrDefaultAsync(c => c.id == request.id , cancellationToken);

            if (category is null)
                return Result.Failure<CategoryDetailsDto>(CategoryErrors.NotFound(request.id));

            return Result<CategoryDetailsDto>.Success(category);
        }
    }
}
