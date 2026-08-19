using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasData(
                new ApplicationRole()
                {
                    Id = 1,
                    Name = "Customer",
                    NormalizedName = "CUSTOMER"
                },
                new ApplicationRole()
                {
                    Id = 2,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                });


        }
    }

}
