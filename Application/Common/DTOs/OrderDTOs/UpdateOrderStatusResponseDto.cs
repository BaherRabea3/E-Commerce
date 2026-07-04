
namespace Application.Common.DTOs.OrderDTOs
{
    public class UpdateOrderStatusResponseDto
    {
        public int OrderId { get; set; }
        public string PreviousStatus { get; set; } = default!;
        public string NewStatus { get; set; } = default!;
        public DateTime UpdatedAt { get; set; }
    }
}
