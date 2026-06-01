using Domain.Entities.Orders;
using Domain.Entities.Payments;
using Domain.Entities.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasOne(x => x.Payment)
                .WithOne(x => x.Order)
                .HasForeignKey<Payment>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Shipment>()
                .WithOne(x => x.Order)
                .HasForeignKey<Shipment>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Orders");
        }
    }
}
