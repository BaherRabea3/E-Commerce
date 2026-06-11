namespace API.Requests.Products
{
    public class GetProductsRequest
    {
        public int? categoryId {  get; set; }
        public int? price { get; set; }
        public string? search {  get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
    }
}
