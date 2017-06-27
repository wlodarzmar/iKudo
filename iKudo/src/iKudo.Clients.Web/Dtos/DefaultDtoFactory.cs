using System;
using System.Collections.Generic;
using AutoMapper;

namespace iKudo.Dtos
{
    public class DefaultDtoFactory : IDtoFactory
    {
        private readonly IMapper mapper;

        public DefaultDtoFactory(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public IEnumerable<TDestination> Create<TDestination, TSource>(IEnumerable<TSource> source)
        {
            return Create<IEnumerable<TDestination>, IEnumerable<TSource>>(source);
        }

        public TDestination Create<TDestination, TSource>(TSource source)
        {
            return mapper.Map<TDestination>(source);
        }

        public object Create<TDestination, TSource>(TSource source, string fields)
        {
            throw new NotImplementedException();
        }
    }
}
