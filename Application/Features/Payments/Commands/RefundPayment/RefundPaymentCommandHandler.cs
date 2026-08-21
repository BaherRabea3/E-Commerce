
using Application.Common.DTOs.PaymentDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Payments;
using Domain.Enums;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Payments.Commands.RefundPayment
{
    public sealed class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, Result<RefundPaymentResponseDto>>
    {
        private readonly IAppDbContext _context;
        private readonly IPaymentGatewayService _paymentService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public RefundPaymentCommandHandler(IAppDbContext context, IPaymentGatewayService payment, IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _paymentService = payment;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Result<RefundPaymentResponseDto>> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments
                                  .Include(p => p.Order)
                                    .ThenInclude(o => o.OrderItems)
                                  .FirstOrDefaultAsync(p => p.Id == request.PaymentId, cancellationToken);

            if (payment is null)
                return Result.Failure<RefundPaymentResponseDto>(PaymentErrors.NotFound);

            if (payment.Status != PaymentStatus.Completed)
                return Result.Failure<RefundPaymentResponseDto>(PaymentErrors.NotRefundable(payment.Status));

            await _paymentService
                .RefundPaymentAsync(payment.GatewayTransactionId,
                                    payment.Amount,
                                    cancellationToken);

            payment.Status = PaymentStatus.Refunded;
            
            if (payment.Order!.Status != OrderStatus.Cancelled)
            {
                payment.Order.Status = OrderStatus.Cancelled;
                payment.Order.CancelledAt = DateTime.UtcNow;
            }

            
            var products = await (from p in _context.Products
                            join oi in _context.OrderItems
                            on p.Id equals oi.ProductId
                            where oi.OrderId == payment.OrderId
                            select p).ToListAsync(cancellationToken);

           foreach (var item in payment.Order.OrderItems)
            {
                var product = products.First(p => p.Id == item.ProductId);

                product.Quantity += item.Quantity;
            }

            await _context.SaveChangesAsync(cancellationToken);

            // send notification
            _backgroundJobClient.Enqueue<IEmailService>(x => x.SendRefundIssuedAsync(payment.OrderId, CancellationToken.None));

            return Result.Success(new RefundPaymentResponseDto()
            {
                OrderId = payment.OrderId,
                PaymentId = payment.Id,
                RefundedAmount = payment.Amount,
                Status = payment.Status.ToString()
            });
        }
    }
}
