
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Categories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Commands.DeleteCategory
{
    public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly IAppDbContext _context;

        public DeleteCategoryCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
           var category = await _context.Categories.FindAsync([request.id], cancellationToken);

            if (category is null)
                return Result.Failure(CategoryErrors.NotFound(request.id));

            bool hasProducts = await _context.Products
                                            .AnyAsync(p => p.CategoryId == category.Id , cancellationToken);

            if (hasProducts)
                return Result.Failure(CategoryErrors.HasProducts(category.Id));

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
