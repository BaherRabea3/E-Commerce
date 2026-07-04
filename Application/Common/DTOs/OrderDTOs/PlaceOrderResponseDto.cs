
namespace Application.Common.DTOs.OrderDTOs
{
    public class PlaceOrderResponseDto
    {
        public int OrderId { get; set; }
        public string? ClientSecret { get; set; } = string.Empty;
    }
}
