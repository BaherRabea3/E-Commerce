using Domain.Entities.Addresses;
using Domain.Entities.Carts;
using Domain.Entities.Orders;
using Domain.Entities.Payments;

namespace Domain.Entities.Customers
{
    public class Customer
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Cart? Cart { get; set; }
        public List<Address> Addresses { get; set; } = new List<Address>();
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Payment> PaymentMethods { get; set; } = new List<Payment>();
    }
}
