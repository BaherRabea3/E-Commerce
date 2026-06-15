
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Categories;
using MediatR;

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
