
using Domain.Common;

namespace Domain.Entities.Addresses
{
    public static class AddressErrors
    {
        public static Error NotFound
            => new("Address.NotFound", "Invalid user address");
    }
}
