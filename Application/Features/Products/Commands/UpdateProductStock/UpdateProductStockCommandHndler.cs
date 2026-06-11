
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Products;
using MediatR;

namespace Application.Features.Products.Commands.UpdateProductStock
{
    public sealed class UpdateProductStockCommandHndler : IRequestHandler<UpdateProductStockCommand , Result>
    {
        private readonly IAppDbContext _context;

        public UpdateProductStockCommandHndler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync([request.id], cancellationToken);

            if (product is null)
                return Result.Failure(ProductErrors.NotFound(request.id));

            product.Quantity = request.quantity;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
