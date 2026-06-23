
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Carts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Carts.Commands.UpdateCartItem
{
    public sealed class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result>
    {
        private readonly IAppDbContext _context;

        public UpdateCartItemCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
        {
           

            var cartItem = await _context.CartItems
                                     .Include(ci => ci.Cart)
                                     .Include(ci => ci.Product)
                                     .FirstOrDefaultAsync(ci => ci.Id == request.cartItemId , cancellationToken);

            if(cartItem is null)
                return Result.Failure(CartErrors.ItemNotFound(request.cartItemId));

            if (cartItem.Cart!.CustomerId != request.customerId)
                return Result.Failure(CartErrors.NotOwned);


            if (request.quantity > cartItem.Product!.Quantity)
                return Result.Failure(CartErrors.InsufficientStock);

            cartItem.Quantity = request.quantity;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
    }
}
