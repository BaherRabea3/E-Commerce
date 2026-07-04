using Domain.Enums;

namespace API.Requests.Orders
{
    public class GetAllOrderRequest
    {
        public int Page {  get; set; }
        public int PageSize { get; set; }
        
        public OrderStatus? Status { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        
        public string? CustomerEmail { get; set; }
        
        public decimal? MinTotal { get; set; }
        public decimal? MaxTotal { get; set; }
    }
}
