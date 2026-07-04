using Domain.Enums;

namespace Application.Common.DTOs.OrderDTOs
{
    public sealed class OrderSummaryDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }

        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = default!;
        public string CustomerEmail { get; set; } = default!;

        public decimal Total { get; set; }
        public string Currency { get; set; } = "USD";

        public PaymentStatus PaymentStatus { get; set; }

        public ShippingStatus? ShipmentStatus { get; set; }  
        public string? TrackingNumber { get; set; }           

        public int ItemCount { get; set; }
    }
}
