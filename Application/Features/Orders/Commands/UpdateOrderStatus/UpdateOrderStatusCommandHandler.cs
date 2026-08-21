using Application.Common.DTOs.OrderDTOs;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Orders;
using Domain.Entities.Payments;
using Domain.Entities.Shipments;
using Domain.Enums;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Commands.UpdateOrderStatus
{
    public sealed class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result<UpdateOrderStatusResponseDto>>
    {
        private readonly IAppDbContext _context;
        private readonly IPaymentGatewayService _paymentService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public UpdateOrderStatusCommandHandler(IAppDbContext context, IPaymentGatewayService paymentService, IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _paymentService = paymentService;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Result<UpdateOrderStatusResponseDto>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                                      .Include(o => o.Payment)
                                      .Include(o => o.Shipment)
                                      .Include(o => o.OrderItems)
                                      .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if(order is null)
                return Result.Failure<UpdateOrderStatusResponseDto>(OrderErrors.NotFound);

            if (!order.CanTransitionTo(request.NewStatus))
                return Result.Failure<UpdateOrderStatusResponseDto>(OrderErrors.InvalidStatusTransition(order.Status, request.NewStatus));

            switch(request.NewStatus)
            {
                case OrderStatus.Shipped:
                     HandleShippedTransition(order);
                    break;
                case OrderStatus.Delivered:
                     HandleDeliveredTransition(order);
                    break;
                case OrderStatus.Cancelled:
                    var cancelResult = await HandleCancelledTransition(order, cancellationToken);
                    if(!cancelResult.IsSuccess)
                        return Result.Failure<UpdateOrderStatusResponseDto>(cancelResult.Error);
                    break;
            }

            var prevStatus = order.Status;
            order.Status = request.NewStatus;

            await _context.SaveChangesAsync(cancellationToken);

            EnqueueNotification(order.Id, request.NewStatus);

            return Result.Success(new UpdateOrderStatusResponseDto()
            {
                OrderId = order.Id,
                PreviousStatus = prevStatus.ToString(),
                NewStatus = order.Status.ToString(),
                UpdatedAt = DateTime.UtcNow
            });
        }

        private async Task<Result> HandleCancelledTransition(Order order, CancellationToken cancellationToken)
        {
            var productIds = order.OrderItems.Select(oi => oi.ProductId).ToList();

            var products = await _context.Products.
                Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            foreach (var item in order.OrderItems)
            {
                var product = products.First(p => p.Id == item.ProductId);

                product.Quantity += item.Quantity;
            }

            if (order.Payment!.Status == PaymentStatus.Completed)
            {
                await _paymentService.RefundPaymentAsync(order.Payment.GatewayTransactionId, order.Total, cancellationToken);

                order.Payment.Status = PaymentStatus.Refunded;
            }

            order.CancelledAt = DateTime.UtcNow;

            return Result.Success();
        }

        private void HandleDeliveredTransition(Order order)
        {
            if (order.Shipment is not null)
            {
                order.Shipment.Status = ShippingStatus.Delivered;
                order.Shipment.ActualDeliveryDate = DateTime.UtcNow;
            }
        }

        private void HandleShippedTransition(Order order)
        {
            if (order.Shipment is null)
            {
                order.Shipment = new Shipment
                {
                    OrderId = order.Id,
                    AddressId = order.AddressId,
                    Status = ShippingStatus.InTransit,
                    EstimatedDeliveryDate = DateTime.UtcNow.AddDays(5)
                };
            }
            else
            {
                order.Shipment.Status = ShippingStatus.InTransit;
            }
        }
        private void EnqueueNotification(int orderId, OrderStatus newStatus)
        {
            switch (newStatus)
            {
                case OrderStatus.Shipped:
                    _backgroundJobClient.Enqueue<IEmailService>(
                        n => n.SendOrderShippedAsync(orderId, CancellationToken.None));
                    break;
                case OrderStatus.Delivered:
                    _backgroundJobClient.Enqueue<IEmailService>(
                        n => n.SendOrderDeliveredAsync(orderId, CancellationToken.None));
                    break;
                case OrderStatus.Cancelled:
                    _backgroundJobClient.Enqueue<IEmailService>(
                        n => n.SendOrderCancelledAsync(orderId, CancellationToken.None));
                    break;
            }
        }
    }
}
