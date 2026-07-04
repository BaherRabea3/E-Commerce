
using Domain.Common;

namespace Domain.Entities.Products
{
    public static class ProductErrors
    {
        public static Error NotFound(int id) 
            => Error.NotFound("Product.NotFound", $"Product with id {id} not found");
        public static Error Discontinued
            => Error.NotFound("Product.Discontinued", "Product does not exist right now");

    }
}
