using Domain.Enums;

namespace Application.Common.DTOs.OrderDTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
    }
}
