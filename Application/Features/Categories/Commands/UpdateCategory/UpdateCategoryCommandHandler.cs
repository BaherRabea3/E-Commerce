
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Categories;
using MediatR;

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

            category.Name = request.name;
            category.Description = request.description;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
