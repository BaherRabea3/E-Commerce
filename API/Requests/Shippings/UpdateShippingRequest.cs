using Domain.Enums;

namespace API.Requests.Shippings
{
    public class UpdateShippingRequest
    {
        public string? ShipmentMethod { get; set; }
        public ShippingStatus? ShippingStatus {  get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
    }
}
