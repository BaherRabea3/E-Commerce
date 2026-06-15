
using Domain.Common;

namespace Domain.Entities.Categories
{
    public static class CategoryErrors
    {
        public static Error NotFound(int id) 
            => Error.NotFound("Category.NotFound", $"Category with id {id} not found");
        public static Error HasProducts(int id) 
            => Error.Conflict("Category.HasProducts", $"Cannot delete a category that has associated products");

    }
}
