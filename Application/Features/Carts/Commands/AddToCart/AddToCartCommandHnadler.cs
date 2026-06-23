using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.CartItems;
using Domain.Entities.Carts;
using Domain.Entities.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Carts.Commands.AddToCart
{
    public sealed class AddToCartCommandHnadler : IRequestHandler<AddToCartCommand, Result<int>>
    {
        private readonly IAppDbContext _context;

        public AddToCartCommandHnadler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync([request.productId], cancellationToken);

            if (product is null)
                return Result.Failure<int>(ProductErrors.NotFound(request.productId));

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerId == request.customerId);

            CartItem? cartItem = null;

            if(cart is null)
            {
                cart = new Cart()
                {
                    CustomerId = request.customerId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
            }
            else
            {
                 cartItem = await _context.CartItems
                                    .FirstOrDefaultAsync(ct => ct.CartId == cart.Id
                                                        && ct.ProductId == product.Id);
            }

                

            var requestedTotalQuantity = (cartItem?.Quantity ?? 0) + request.quantity;

            if (requestedTotalQuantity > product.Quantity)
                return Result.Failure<int>(CartErrors.InsufficientStock);

            if (cartItem is null)
            {
                cartItem = new CartItem()
                {
                    Cart =cart,
                    ProductId = product.Id,
                    Quantity = request.quantity
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity = requestedTotalQuantity;
            }


            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(cart.Id);
           
        }
    }
}
