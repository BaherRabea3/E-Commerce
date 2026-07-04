using Domain.Entities.Payments;
using Domain.Entities.Customers;
using Domain.Entities.OrderItems;
using Domain.Enums;
using Domain.Entities.Addresses;
using Domain.Entities.Shipments;

namespace Domain.Entities.Orders
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime CancelledAt { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; }
        public Guid IdempotencyKey { get; set; }
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public Shipment? Shipment { get; set; }
        public Payment? Payment { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public int AddressId { get; set; }
        public Address? Address { get; set; }

        private static readonly Dictionary<OrderStatus, IReadOnlyList<OrderStatus>> AllowedTransitions = new()
        {
            [OrderStatus.AwaitingPayment] = new[] { OrderStatus.Cancelled },
            [OrderStatus.Confirmed] = new[] { OrderStatus.Shipped, OrderStatus.Cancelled },
            [OrderStatus.Shipped] = new[] { OrderStatus.Delivered },
            [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
            [OrderStatus.Cancelled] = Array.Empty<OrderStatus>()
        };
        public bool CanTransitionTo(OrderStatus newStatus)
            => AllowedTransitions.TryGetValue(Status, out var allowed)
               && allowed.Contains(newStatus);
    }
}
