using Domain.Entities.Addresses;
using Domain.Entities.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {

            builder.HasOne<Shipment>()
                .WithOne(x => x.Address)
                .HasForeignKey<Shipment>(x => x.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Addresses");
        }
    }
}
