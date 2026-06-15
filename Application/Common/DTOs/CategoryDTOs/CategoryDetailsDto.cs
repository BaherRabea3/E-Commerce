
namespace Application.Common.DTOs.CategoryDTOs
{
    public class CategoryDetailsDto
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public int productsCount { get; set; }
    }
}
