using Domain.Enums;

namespace API.Requests.Orders
{
    public class GetCustomerOrdersRequest
    {
        public int? page {  get; set; }
        public int? pageSize { get; set; }

        public OrderStatus? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
