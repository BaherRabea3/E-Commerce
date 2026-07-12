using Domain.Entities.Orders;
using Domain.Entities.Customers;
using Domain.Enums;

namespace Domain.Entities.Payments
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PaidAt { get; set; }
        public string Method { get; set; } = default!;
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string GatewayTransactionId { get; set; } = default!;
        public Guid CustomerId { get; set; }
        public int OrderId { get; set; }
        public Customer? Customer { get; set; }
        public Order? Order { get; set; }

        private static Dictionary<PaymentStatus, IReadOnlyList<PaymentStatus>> AllowedTransitions
            => new()
            {
                [PaymentStatus.Pending] = new[] {PaymentStatus.Completed, PaymentStatus.Failed},
                [PaymentStatus.Completed] = new[] { PaymentStatus.Refunded },
                [PaymentStatus.Failed] = new[] { PaymentStatus.Pending },
                [PaymentStatus.Refunded] = Array.Empty<PaymentStatus>(),
            };

        public bool CanTransitionTo(PaymentStatus newStatus)
            => AllowedTransitions.TryGetValue(Status , out var allowed)
            && allowed.Contains(newStatus);

    }

}
