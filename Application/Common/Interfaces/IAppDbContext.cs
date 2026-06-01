using Domain.Entities.Addresses;
using Domain.Entities.CartItems;
using Domain.Entities.Carts;
using Domain.Entities.Categories;
using Domain.Entities.Customers;
using Domain.Entities.OrderItems;
using Domain.Entities.Orders;
using Domain.Entities.Payments;
using Domain.Entities.Products;
using Domain.Entities.Shipments;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IAppDbContext
    {
         DbSet<Customer> Customers { get; }
         DbSet<Product> Products { get; }
         DbSet<Order> Orders { get; }
         DbSet<Shipment> Shipments { get; }
         DbSet<OrderItem> OrderItems { get; }
         DbSet<Payment> Payments { get; }
         DbSet<Address> Addresses { get; }
         DbSet<Cart> Carts { get; }
         DbSet<CartItem> CartItems { get; }
         DbSet<Category> Categories { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
