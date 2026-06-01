using Domain.Entities.Customers;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; } = default!;
        public Customer? Customer { get; set; }
    }
}
