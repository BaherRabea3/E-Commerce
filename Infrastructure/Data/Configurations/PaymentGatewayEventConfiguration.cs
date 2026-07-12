using Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PaymentGatewayEventConfiguration
    : IEntityTypeConfiguration<PaymentGatewayEvent>
    {
        public void Configure(EntityTypeBuilder<PaymentGatewayEvent> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.GatewayEventId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.EventType)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(e => e.GatewayEventId)
                .IsUnique();

            builder.ToTable("PaymentGatewayEvents");
        }
    }
}
