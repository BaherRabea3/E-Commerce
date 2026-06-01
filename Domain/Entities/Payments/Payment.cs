using Domain.Entities.Orders;
using Domain.Entities.Customers;

namespace Domain.Entities.Payments
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Method { get; set; } = default!;
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public Guid CustomerId { get; set; }
        public int OrderId { get; set; }
        public Customer? Customer { get; set; }
        public Order? Order { get; set; }
    }

}
