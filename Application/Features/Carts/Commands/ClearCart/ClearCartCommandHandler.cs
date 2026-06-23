using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Carts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Carts.Commands.ClearCart
{
    public sealed class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Result>
    {
        private readonly IAppDbContext _context;

        public ClearCartCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _context.Carts
                               .Include(c => c.CartItems)                
                               .FirstOrDefaultAsync(c => c.CustomerId == request.customerId , cancellationToken);

            if (cart is null)
                return Result.Failure(CartErrors.NotFound);

            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
