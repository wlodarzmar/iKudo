using System.Collections.Generic;

namespace iKudo.Dtos
{
    public interface IDtoFactory
    {
        TDestination Create<TDestination, TSource>(TSource source);

        IEnumerable<TDestination> Create<TDestination, TSource>(IEnumerable<TSource> source);

        object Create<TDestination, TSource>(TSource source, string fields);
    }
}
