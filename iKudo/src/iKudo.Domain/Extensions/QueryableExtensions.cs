using System;
using System.Linq;
using System.Linq.Expressions;

namespace iKudo.Domain.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> collection, Expression<Func<T, bool>> predicate, bool condition)
        {
            if (condition)
            {
                collection = collection.Where(predicate);
            }

            return collection;
        }
    }
}
