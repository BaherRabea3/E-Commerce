
namespace Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderConfirmedAsync(int orderId, CancellationToken cancellationToken);
        Task SendOrderCancelledAsync(int orderId, CancellationToken cancellationToken);
        Task SendOrderShippedAsync(int orderId, CancellationToken cancellationToken);
        Task SendOrderDeliveredAsync(int orderId, CancellationToken cancellationToken);
        Task SendPaymentFailedAsync(int orderId, CancellationToken cancellationToken);
        Task SendRefundIssuedAsync(int orderId, CancellationToken cancellationToken);
    }
}
