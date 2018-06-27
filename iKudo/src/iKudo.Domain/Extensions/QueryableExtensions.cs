using System;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

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

        public static IQueryable<T> OrderByIf<T>(this IQueryable<T> collection, string order, bool condition)
        {
            if (condition)
            {
                collection = collection.OrderBy(order);
            }

            return collection;
        }
    }
}
