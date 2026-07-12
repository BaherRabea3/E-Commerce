
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Shipments;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.Commands.ProcessStripeWebhook
{
    public sealed class ProcessStripeWebhookCommandHandler : IRequestHandler<ProcessStripeWebhookCommand, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IPaymentGatewayService _payment;
        private readonly ILogger<ProcessStripeWebhookCommandHandler> _logger;

        public ProcessStripeWebhookCommandHandler(IAppDbContext context, ILogger<ProcessStripeWebhookCommandHandler> logger, IPaymentGatewayService payment)
        {
            _context = context;
            _logger = logger;
            _payment = payment;
        }

        public async Task<Result> Handle(ProcessStripeWebhookCommand request, CancellationToken cancellationToken)
        {
            bool alreadyProcessed = await _context.PaymentGatewayEvents
                                            .AnyAsync(x => x.GatewayEventId == request.EventId, cancellationToken);
            if (alreadyProcessed)
            {
                _logger.LogInformation(
                   "Webhook event {EventId} already processed — skipping.",
                   request.EventId);
                return Result.Success();
            }

            switch (request.EventType)
            {
                case "payment_intent.succeeded":
                    await HandlePaymentSucceeded(request.RawJson, cancellationToken);
                    break;

                case "payment_intent.payment_failed":
                    await HandlePaymentFailed(request.RawJson, cancellationToken);
                    break;

                case "charge.refunded":
                    await HandleRefundConfirmed(request.RawJson, cancellationToken);
                    break;

                default:
                    _logger.LogInformation(
                        "Unhandled Stripe event type: {EventType}", request.EventType);
                    break;
            }

            return Result.Success();
        }

        private async Task HandleRefundConfirmed(string rawJson, CancellationToken cancellationToken)
        {
            var TransactionId = _payment.GetGatewayTransactionId(rawJson);

            var payment = await _context.Payments
                                        .FirstOrDefaultAsync(p => p.GatewayTransactionId == TransactionId, cancellationToken);

            if (payment is null || payment.Status == PaymentStatus.Refunded)
                return;

            payment.Status = PaymentStatus.Refunded;

            await _context.SaveChangesAsync(cancellationToken);

        }

        private async Task HandlePaymentFailed(string rawJson, CancellationToken cancellationToken)
        {
            var TransactionId = _payment.GetGatewayTransactionId(rawJson);

            var payment = await _context.Payments
                                        .FirstOrDefaultAsync(p => p.GatewayTransactionId == TransactionId, cancellationToken);

            if (payment is null || payment.Status == PaymentStatus.Failed)
                return;

            payment.Status = PaymentStatus.Failed;

            await _context.SaveChangesAsync(cancellationToken);

            // send notification
        }

        private async Task HandlePaymentSucceeded(string rawJson, CancellationToken cancellationToken)
        {
            var TransactionId = _payment.GetGatewayTransactionId(rawJson);

            var payment = await _context.Payments
                .Include(p => p.Order)                    
                .FirstOrDefaultAsync(p => p.GatewayTransactionId ==  TransactionId , cancellationToken);

            if (payment is null)
            {
                _logger.LogWarning(
                    "No Payment found for GatewayTransactionId {Id} — possible webhook race.",
                    TransactionId);
                return;
            }

            if (payment.Status == PaymentStatus.Completed)
            {
                _logger.LogInformation(
                    "Payment {Id} already confirmed — skipping.", payment.Id);
                return;
            }

            payment.Status = PaymentStatus.Completed;
            payment.PaidAt = DateTime.UtcNow;

            payment.Order!.Status = OrderStatus.Confirmed;

            var shipmentExisted = await _context.Shipments
                .AnyAsync(s => s.OrderId == payment.OrderId, cancellationToken);

            if (!shipmentExisted)
            {
                var shipment = new Shipment()
                {
                    OrderId = payment.OrderId,
                    AddressId = payment.Order.AddressId,
                    EstimatedDeliveryDate = DateTime.UtcNow.AddDays(5),
                    Status = ShippingStatus.InTransit,
                    Method = "Standard"
                };
            }

            await _context.SaveChangesAsync(cancellationToken);

            // send notification

        }
    }
}
