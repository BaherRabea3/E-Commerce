namespace API.Requests.Addresses
{
    public class CreateAddressRequest
    {
      public string State { get; set; } = string.Empty;
      public string PostalCode { get; set; } = string.Empty;
      public string HouseNo    { get; set; } = string.Empty;
      public string Street     { get; set; } = string.Empty;
      public string Area       { get; set; } = string.Empty;
      public string Province   { get; set; } = string.Empty;
      public string City       { get; set; } = string.Empty;
      public string Country { get; set; }    = string.Empty;
    }
}
