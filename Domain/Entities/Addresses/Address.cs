using Domain.Entities.Customers;

namespace Domain.Entities.Addresses
{
    public class Address
    {
        public int Id { get; set; }
        public string state { get; set; } = default!;
        public string city { get; set; } = default!;
        public string country { get; set; } = default!;
        public string PostalCode { get; set; } = default!;
        public string HouseNo { get; set; } = default!;
        public string StreetBlock { get; set; } = default!;
        public string Area { get; set; } = default!;
        public string Province { get; set; } = default!;
        public Guid CustomerId { get; set; } = default!;
        public Customer Customer { get; set; } = default!;
    }
}
