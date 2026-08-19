
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Categories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Commands.UpdateCategory
{
    public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
    {
        private readonly IAppDbContext _context;

        public UpdateCategoryCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FindAsync([request.id], cancellationToken);

            if (category is null)
                return Result.Failure(CategoryErrors.NotFound(request.id));

            var isExisted = await _context.Categories.AnyAsync(x => x.Name == request.name && x.Id != category.Id, cancellationToken);

            if (isExisted)
                return Result.Failure<int>(CategoryErrors.DuplicateName);

            category.Name = request.name;
            category.Description = request.description;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
