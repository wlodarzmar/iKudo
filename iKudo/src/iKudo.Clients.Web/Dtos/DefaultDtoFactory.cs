using System.Collections.Generic;
using AutoMapper;
using System.Dynamic;
using System.Reflection;

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
            if (string.IsNullOrWhiteSpace(fields))
            {
                return Create<TDestination, TSource>(source);
            }
            else
            {
                return CreateDynamicObject(source, fields);
            }
        }

        private object CreateDynamicObject<TSource>(TSource source, string fields)
        {
            var fieldsList = fields.ToLower().Split(',');

            ExpandoObject result = new ExpandoObject();
            foreach (var field in fieldsList)
            {
                object value = source.GetType()
                                     .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)
                                     .GetValue(source);

                ((IDictionary<string, object>)result).Add(field, value);
            }

            return result;
        }
    }
}
