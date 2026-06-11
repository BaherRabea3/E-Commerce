using Microsoft.AspNetCore.Http;

namespace Application.Common.DTOs.ProductDTOs
{
    public class ProductDto
    {
        public int id {  get; set; }
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public decimal unitPrice { get; set; }
        public int quantity { get; set; }
        public string imageUrl { get; set; } = string.Empty;
        public string category {  get; set; } = string.Empty;
    }
}
