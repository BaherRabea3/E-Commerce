
namespace Application.Common.DTOs.PaymentDTOs
{
    public class PaymentSummartDto
    {
        public string Method { get; set; } = default!;
        public string Status { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
