
using Domain.Common;

namespace Domain.Entities.Carts
{
    public static class CartErrors
    {
        public static Error NotFound
            => Error.NotFound("Cart.NotFound", $"Cart was not found");
        public static Error ItemNotFound(int id)
            => Error.NotFound("CartItem.NotFound", $"Cart item with id {id} was not found");
        public static Error InsufficientStock
            => Error.UnprocessableEntity("CartItem.InsufficientStock", "Requested quantity exceeds available stock");
        public static Error NotOwned
            => Error.NotFound("CartItem.NotOwned", "This cart item does not belong to you");
        public static Error IsEmpty
            => Error.NotFound("Cart.IsEmpty", "Cart is empty");
    }
}
