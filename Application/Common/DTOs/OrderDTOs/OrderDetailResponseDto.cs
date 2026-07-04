
using Application.Common.DTOs.AddressDTOs;
using Application.Common.DTOs.PaymentDTOs;
using Application.Common.DTOs.ShipmentDTOs;

namespace Application.Common.DTOs.OrderDTOs
{
    public class OrderDetailResponseDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = default!;

        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
        public ShippingAddressDto Address { get; set; } = default!;
        public PaymentSummartDto Payment { get; set; } = default!;
        public ShipmentSummaryDto? Shipment { get; set; }

    }
}
