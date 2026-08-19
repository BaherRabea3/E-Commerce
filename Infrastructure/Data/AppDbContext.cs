using Application.Common.Interfaces;
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
using Infrastructure.Data.Configurations;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Shipment> Shipments { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<PaymentGatewayEvent> PaymentGatewayEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(CustomerConfiguration).Assembly);
        }

    }
}
