
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Categories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Commands.CreateCategory
{
    public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<int>>
    {
        private readonly IAppDbContext _context;

        public CreateCategoryCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {

            var isExisted = await _context.Categories.AnyAsync(x => x.Name == request.name, cancellationToken);

            if (isExisted)
                return Result.Failure<int>(CategoryErrors.DuplicateName);

            var category = new Category
            {
                Name = request.name,
                Description = request.description
            };

            _context.Categories.Add(category);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(category.Id);
        }
    }
}
