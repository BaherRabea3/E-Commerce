

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
            var cartItem = await _context.CartItems
                            .FirstOrDefaultAsync(ci => 
                                        ci.Cart!.CustomerId == request.customerId &&
                                        ci.Id == request.id, cancellationToken);

            if(cartItem is null)
                return Result.Failure(CartErrors.ItemNotFound(request.id));

            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
