

namespace Application.Common.DTOs.CartDTOs
{
    public class CartDetailsDto
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItemDetailsDto> CartItems { get; set; } = new List<CartItemDetailsDto>();
    }
}
