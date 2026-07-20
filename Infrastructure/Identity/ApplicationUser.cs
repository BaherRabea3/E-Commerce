using Domain.Entities.Customers;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        public string? RefreshToken { get; set; } = default!;

        public DateTime? RefreshTokenExpiration {  get; set; } = default!;

        public Customer? Customer { get; set; }
    }
}
