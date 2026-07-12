
namespace Application.Common.DTOs.PaymentDTOs
{
    public class UpdatePaymentStatusResponseDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string PreviousStatus { get; set; } = default!;
        public string NewStatus { get; set; } = default!;
        public DateTime UpdatedAt { get; set; }
    }
}
