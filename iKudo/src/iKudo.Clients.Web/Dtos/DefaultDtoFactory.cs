using System;
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

        public TDestination Create<TDestination, TSource>(TSource source)
        {
            return mapper.Map<TDestination>(source);
        }
    }
}
