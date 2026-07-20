
namespace Application.Common.DTOs.ShipmentDTOs
{
    public class ShipmentDetailsDto
    {
        public int ShipmentId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Method { get; set; }
        public string Status { get; set; } = default!;
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
    }
}
