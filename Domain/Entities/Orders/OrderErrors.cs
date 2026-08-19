
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Orders
{
    public static class OrderErrors
    {
        public static Error NotFound 
            => Error.NotFound("Order.NotFound", "No Order found");
        public static Error CustomerHasNoOrders
           => Error.NotFound("Order.CustomerHasNoOrders", "Customer Hasn't order yet");
        public static Error NotCancellable
            => Error.Conflict("Order.NotCancellable", "Order not cancellable");

        public static Error InvalidStatusTransition(OrderStatus current, OrderStatus attempted) =>
        Error.Conflict("Order.InvalidStatusTransition",
            $"Cannot transition order from '{current}' to '{attempted}'.");
    }
}
