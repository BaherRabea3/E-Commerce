using Domain.Entities.Orders;
using Domain.Entities.Products;

namespace Domain.Entities.OrderItems
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quatity { get; set; }
        public decimal Price { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
        
    }
}
