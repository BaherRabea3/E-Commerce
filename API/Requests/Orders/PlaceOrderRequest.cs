namespace API.Requests.Orders
{
    public class PlaceOrderRequest
    {
        public Guid IdempotencyKey { get; set; }
        public int AddressId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
