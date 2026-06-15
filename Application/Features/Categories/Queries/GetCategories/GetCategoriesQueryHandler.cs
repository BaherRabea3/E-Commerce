
using Application.Common.DTOs.CategoryDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Queries.GetCategories
{
    public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDTO>>>
    {
        private readonly IAppDbContext _context;

        public GetCategoriesQueryHandler(IAppDbContext Context)
        {
            _context = Context;
        }

        public async Task<Result<List<CategoryDTO>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categoryList = await _context.Categories
                                             .AsNoTracking()
                                             .Select(c => new CategoryDTO
                                             {
                                                 id = c.Id,
                                                 name = c.Name,
                                                 description = c.Description
                                             })
                                             .ToListAsync(cancellationToken);
            return Result<List<CategoryDTO>>.Success(categoryList);
        }
    }
}
