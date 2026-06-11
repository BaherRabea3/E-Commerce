
namespace Application.Common.DTOs
{
    public class PaginatedResult<T>
    {
        public int page {  get; set; }
        public int pageSize { get; set; }

        public int totalCount { get; set; }
        public int totalPages => (int)Math.Ceiling((double)totalCount / pageSize);

        public bool HasPrevious => page > 1;
        public bool HasNext => page < totalPages;
        public IEnumerable<T> items { get; set; } = Enumerable.Empty<T>();
       
    }
}
