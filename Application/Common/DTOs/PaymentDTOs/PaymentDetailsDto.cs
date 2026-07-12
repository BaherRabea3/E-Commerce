
namespace Application.Common.DTOs.PaymentDTOs
{
    public class PaymentDetailsDto
    {
        public int PaymentId { get; set; }
        public string Method { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
