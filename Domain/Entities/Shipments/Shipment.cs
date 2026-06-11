using Domain.Entities.Addresses;
using Domain.Entities.Orders;
using Domain.Enums;

namespace Domain.Entities.Shipments
{
    public class Shipment
    {
        public int Id { get; set; }
        public string city { get; set; } = default!;
        public DateTime Date { get; set; }
        public DateTime DeliveryDate { get; set; } 
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public string Method { get; set; } = default!;
        public ShippingStatus Status { get; set; }
        public int AddressId { get; set; }
        public Address? Address { get; set; }

    }
}
