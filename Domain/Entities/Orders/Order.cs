using Domain.Entities.Payments;
using Domain.Entities.Customers;
using Domain.Entities.OrderItems;

namespace Domain.Entities.Orders
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public Payment? Payment { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
