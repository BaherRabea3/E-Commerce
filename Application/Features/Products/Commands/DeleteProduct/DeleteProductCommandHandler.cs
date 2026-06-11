
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Categories;
using Domain.Entities.Products;
using MediatR;

namespace Application.Features.Products.Commands.DeleteProduct
{
    public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand , Result>
    {
        private readonly IAppDbContext _context;

        public DeleteProductCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync([request.id], cancellationToken);

            if (product is null)
                return Result.Failure(ProductErrors.NotFound(request.id));

            _context.Products.Remove(product);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
