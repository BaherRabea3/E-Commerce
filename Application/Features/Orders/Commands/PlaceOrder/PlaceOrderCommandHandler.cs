using Application.Common.DTOs.OrderDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Addresses;
using Domain.Entities.Carts;
using Domain.Entities.OrderItems;
using Domain.Entities.Orders;
using Domain.Entities.Payments;
using Domain.Entities.Products;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Commands.PlaceOrder
{
    public sealed class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Result<PlaceOrderResponseDto>>
    {
        private readonly IAppDbContext _context;
        private readonly IPaymentGatewayService _paymentService;

        public PlaceOrderCommandHandler(IAppDbContext context, IPaymentGatewayService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        public async Task<Result<PlaceOrderResponseDto>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {

            var existingOrder = await _context.Orders
                                .Include(o => o.Payment)
                                .FirstOrDefaultAsync(o => o.IdempotencyKey == request.IdempotencyKey , cancellationToken);

            if (existingOrder is not null)
            {
                string? clientSecret = null;

                if (existingOrder.Payment?.GatewayTransactionId is not null)
                {
                    var paymentIntent = await _paymentService.GetClientSecretAsync(existingOrder.Payment.GatewayTransactionId, cancellationToken);

                    clientSecret = paymentIntent.ClientSecret;
                }
                
                return Result.Success(MapToResponseDto(existingOrder, clientSecret));
            }

            var cart = _context.Carts
                                .Include(c => c.CartItems)
                                .FirstOrDefault(c => c.CustomerId == request.customerId);

            if(cart is null)
                return Result.Failure<PlaceOrderResponseDto>(CartErrors.NotFound);

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == request.addressId &&
                                          a.CustomerId == request.customerId,
                                          cancellationToken);
            if (address is null)
                return Result.Failure<PlaceOrderResponseDto>(AddressErrors.NotFound);

            if (!cart.CartItems.Any())
                return Result.Failure<PlaceOrderResponseDto>(CartErrors.IsEmpty);

            var productIds = cart.CartItems.Select(ci => ci.ProductId).ToList();
            var products = (from p in _context.Products
                           join id in productIds
                           on p.Id equals id
                           select p).ToList();

            foreach (var cartItem in cart.CartItems)
            {
                var product = products.FirstOrDefault(p => p.Id == cartItem.ProductId);

                if(product is null)
                    return Result.Failure<PlaceOrderResponseDto>(ProductErrors.NotFound(cartItem.ProductId));

                if(product.IsDeleted)
                    return Result.Failure<PlaceOrderResponseDto>(ProductErrors.Discontinued);

                if(cartItem.Quantity > product.Quantity)
                    return Result.Failure<PlaceOrderResponseDto>(CartErrors.InsufficientStock);

            }

            var order = new Order
            {
                CustomerId = request.customerId,
                AddressId = request.addressId,
                IdempotencyKey = request.IdempotencyKey,
                Date = DateTime.UtcNow,
                Status = OrderStatus.AwaitingPayment
            };

            decimal subTotal = 0;

            foreach (var cartItem in cart.CartItems)
            {
                var product = products.First(p => p.Id == cartItem.ProductId);

                order.OrderItems.Add(new OrderItem()
                {
                    ProductId = product.Id,
                    Price = cartItem.UnitPrice,
                    Quantity = cartItem.Quantity
                });

                subTotal += cartItem.UnitPrice * cartItem.Quantity;

                product.Quantity -= cartItem.Quantity;
            }
            var Tax = CalculateTax(subTotal, address);
            var ShippingCosts = CalculateShipping(address);

            order.ShippingCost = ShippingCosts;
            order.Tax = Tax;
            order.Subtotal = subTotal;
            order.Total = subTotal + Tax + ShippingCosts;

            order.Payment = new Payment()
            {
                CustomerId = request.customerId,
                CreatedAt = DateTime.UtcNow,
                Method = request.paymentMethod,
                Status = PaymentStatus.Pending,
                Amount = order.Total,
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cart.CartItems);

            // Optimistic lock for race condition problem
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Failure<PlaceOrderResponseDto>(CartErrors.InsufficientStock);
            }

            var PaymentIntentResult = await _paymentService.CreatePaymentIntentAsync(order.Total, "usd", order.Id, cancellationToken);


            order.Payment.GatewayTransactionId = PaymentIntentResult.GatewayTransactionId!;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(MapToResponseDto(order, PaymentIntentResult.ClientSecret));
        }

        private PlaceOrderResponseDto MapToResponseDto(Order order, string? clientSecret)
        => new PlaceOrderResponseDto()
        {
            OrderId = order.Id,
            ClientSecret = clientSecret
        };

        private static decimal CalculateShipping(Address address)
        {
            // TODO: replace with real shipping-rule lookup
            return 5.00m;
        }
        private static decimal CalculateTax(decimal subtotal, Address address)
        {
            // TODO: replace with real tax service
            return subtotal * 0.05m;
        }
    }
}
