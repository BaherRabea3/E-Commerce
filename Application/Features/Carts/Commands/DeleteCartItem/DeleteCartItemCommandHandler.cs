

using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Carts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Carts.Commands.DeleteCartItem
{
    public class DeleteCartItemCommandHandler : IRequestHandler<DeleteCartItemCommand, Result>
    {
        private readonly IAppDbContext _context;

        public DeleteCartItemCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
        {
            var Customer = await _context.Customers.FirstAsync(x => x.UserId == request.customerId, cancellationToken);

            var cart = await _context.Carts.FirstAsync(x => x.CustomerId == Customer.Id, cancellationToken);

            var cartItem = await _context.CartItems
                            .FirstOrDefaultAsync(ci => 
                                        cart.CustomerId == Customer.Id &&
                                        ci.Id == request.id, cancellationToken);

            if(cartItem is null)
                return Result.Failure(CartErrors.ItemNotFound(request.id));

            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
