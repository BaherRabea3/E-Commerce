using Domain.Entities.Addresses;
using Domain.Entities.Orders;
using Domain.Enums;

namespace Domain.Entities.Shipments
{
    public class Shipment
    {
        public int Id { get; set; }
        public string city { get; set; } = default!;
        public string? TrackingNumber { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public DateTime ActualDeliveryDate { get; set; } 
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public string Method { get; set; } = default!;
        public ShippingStatus Status { get; set; }
        public int AddressId { get; set; }
        public Address? Address { get; set; }

        private static Dictionary<ShippingStatus, IReadOnlyList<ShippingStatus>> AllowedTransitions => new()
        {
            [ShippingStatus.Pending] =
            new[] { ShippingStatus.Delivered, ShippingStatus.Cancelled, ShippingStatus.InTransit },

            [ShippingStatus.InTransit] =
            new[] { ShippingStatus.Delivered, ShippingStatus.Cancelled },

            [ShippingStatus.Delivered] = Array.Empty<ShippingStatus>(),
            [ShippingStatus.Cancelled] = Array.Empty<ShippingStatus>()


        };

        public bool CanTransitionTo(ShippingStatus NewshippingStatus) =>
            AllowedTransitions.TryGetValue(Status, out var allowed)
            && allowed.Contains(NewshippingStatus);


    }
}
