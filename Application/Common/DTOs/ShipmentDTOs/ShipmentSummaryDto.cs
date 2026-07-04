
namespace Application.Common.DTOs.ShipmentDTOs
{
    public class ShipmentSummaryDto
    {
        public string? TrackingNumber { get; set; }
        public string? Method { get; set; }
        public string Status { get; set; } = default!;
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
    }
}
