
using Application.Common.DTOs.CartDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Carts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Carts.Queries.GetCart
{
    public sealed class GetCartQueryHandler : IRequestHandler<GetCartQuery, Result<CartDetailsDto>>
    {
        private readonly IAppDbContext _context;

        public GetCartQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CartDetailsDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _context.Carts
                               .AsNoTracking()
                               .Include(c => c.CartItems)
                                    .ThenInclude(ci => ci.Product)
                               .FirstOrDefaultAsync(c => c.CustomerId == request.customerId , cancellationToken);
            if (cart is null)
                return Result.Failure<CartDetailsDto>(CartErrors.NotOwned);

            var result = new CartDetailsDto
            {
                Id = cart.Id,
                TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.Product!.UnitPrice),
                CartItems = cart.CartItems
                                .Where(ci => ci.Product != null)
                                .Select(ci => new CartItemDetailsDto()
                                {
                                    Id = ci.Id,
                                    ProductId = ci.ProductId,
                                    ProductName = ci.Product!.Name,
                                    ProductPrice = ci.Product.UnitPrice,
                                    Quantity = ci.Quantity
                                }).ToList()
            };

            return Result.Success(result);
        }
    }
}
