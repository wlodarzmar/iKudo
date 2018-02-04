using System.Collections.Generic;

namespace iKudo.Dtos
{
    public interface IDtoFactory
    {
        TDestination Create<TDestination, TSource>(TSource source);

        IEnumerable<TDestination> Create<TDestination, TSource>(IEnumerable<TSource> source);

        TDestination Map<TDestination, TSource>(TDestination destination, TSource source);

        TDestination Create<TDestination, TSource>(TSource source, string fields);
    }
}
