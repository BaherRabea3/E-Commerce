using Domain.Entities.Carts;
using Domain.Entities.Products;

namespace Domain.Entities.CartItems
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CartId  { get; set; }
        public int Quantity { get; set; }
        public Product? Product { get; set; }
        public Cart? Cart { get; set; }
    }
}
