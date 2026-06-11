using Domain.Entities.CartItems;
using Domain.Entities.Categories;
using Domain.Entities.OrderItems;
using Domain.Interfaces.Common;

namespace Domain.Entities.Products
{
    public class Product : ISoftDeleteable
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? Image {  get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public bool IsDelete { get ; set ; }
        public DateTime? DateDeleted { get ; set ; }
    }
}
