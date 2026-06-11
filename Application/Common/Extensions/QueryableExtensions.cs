using System.Linq.Expressions;

namespace Application.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> ApplyPagination<TSource>(this IQueryable<TSource> source,
            int? page, int? pageSize)
        {
            page = page ?? 1;
            pageSize = pageSize ?? 8;

            return source.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        public static IQueryable<TSource> ApplyFilter<TSource>(this IQueryable<TSource> source
            , Expression<Func<TSource, bool>> predicate )
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            return source.Where(predicate);
        }

        public static IQueryable<TSource> FilterWithPaginate<TSource>(this IQueryable<TSource> source
            , Expression<Func<TSource, bool>> predicate
            , int? page , int? pageSize)
        {
            if(source is null)
                throw new ArgumentNullException(nameof(source));

            if(predicate is null)
                throw new ArgumentNullException(nameof(predicate));
            
            var query = source.Where(predicate);

            return query.ApplyPagination(page , pageSize);
        }
    }
}
