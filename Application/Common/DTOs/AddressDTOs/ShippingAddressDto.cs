
namespace Application.Common.DTOs.AddressDTOs
{
    public class ShippingAddressDto
    {
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string PostalCode { get; set; } = default!;
        public string HouseNo { get; set; } = default!;
        public string StreetBlock { get; set; } = default!;
        public string Area { get; set; } = default!;
    }
}
