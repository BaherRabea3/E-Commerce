using Domain.Entities.CartItems;
using Domain.Entities.Customers;

namespace Domain.Entities.Carts
{
    public class Cart
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
