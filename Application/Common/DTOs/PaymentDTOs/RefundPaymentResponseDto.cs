
namespace Application.Common.DTOs.PaymentDTOs
{
    public class RefundPaymentResponseDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal RefundedAmount { get; set; }
        public string Status { get; set; } = default!;
    }
}
