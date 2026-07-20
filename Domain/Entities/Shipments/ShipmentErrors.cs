
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Shipments
{
    public static class ShipmentErrors
    {
        public static Error NotFound
            => Error.NotFound("Shipment.NotFound", "Shipment not found");

        public static Error InvalidStatusTransition(ShippingStatus current, ShippingStatus attempted)
            => Error.Conflict(
        "Shipment.InvalidStatusTransition",
        $"Cannot transition Shipping from '{current}' to '{attempted}'.");
    }
}
