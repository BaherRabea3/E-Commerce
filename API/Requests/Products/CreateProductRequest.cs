namespace API.Requests.Products
{
    public class CreateProductRequest
    {
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public decimal unitPrice { get; set; }
        public int quantity { get; set; }
        public IFormFile Image { get; set; } = default!;
        public int categoryId { get; set; }
    }
}
