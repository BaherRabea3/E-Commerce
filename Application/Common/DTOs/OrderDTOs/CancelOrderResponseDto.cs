
namespace Application.Common.DTOs.OrderDTOs
{
    public class CancelOrderResponseDto
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public bool RefundIssued { get; set; }
        public decimal RefundAmount { get; set; }
    }
}
