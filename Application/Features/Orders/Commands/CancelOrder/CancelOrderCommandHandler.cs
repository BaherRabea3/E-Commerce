using Application.Common.DTOs.OrderDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Orders;
using Domain.Entities.Payments;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Features.Orders.Commands.CancelOrder
{
    public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<CancelOrderResponseDto>>
    {
        private readonly IAppDbContext _context;
        private readonly IPaymentGatewayService _paymentService;

        public CancelOrderCommandHandler(IAppDbContext context, IPaymentGatewayService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        public async Task<Result<CancelOrderResponseDto>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {

            var customer = await _context.Customers.FirstAsync(x => x.UserId == request.CustomerId);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId &&
                                          o.CustomerId == customer.Id, cancellationToken);

            if (order is null)
                return Result.Failure<CancelOrderResponseDto>(OrderErrors.NotFound);

            if (order.Status != OrderStatus.AwaitingPayment && order.Status != OrderStatus.Confirmed)
                return Result.Failure<CancelOrderResponseDto>(OrderErrors.NotCancellable);

            var wasConfirmed = order.Status == OrderStatus.Confirmed;

            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FirstAsync(x => x.Id == item.ProductId, CancellationToken.None);

                product.Quantity += item.Quantity;
            }

            decimal refundAmount = 0;

            if (wasConfirmed)
            {
                 await _paymentService.RefundPaymentAsync(
                    order.Payment!.GatewayTransactionId, order.Total, cancellationToken);

                order.Payment.Status = PaymentStatus.Refunded;

                refundAmount = order.Total;
            }

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // send email notification to customer

            return Result.Success(new CancelOrderResponseDto()
            {
                OrderId = order.Id,
                OrderStatus = order.Status.ToString(),
                RefundAmount = refundAmount,
                RefundIssued = wasConfirmed
            });
        }


    }
}
