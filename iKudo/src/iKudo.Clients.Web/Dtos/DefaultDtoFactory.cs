using AutoMapper;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
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

        public TDestination Map<TDestination, TSource>(TDestination destination, TSource source)
        {
            return mapper.Map(source, destination);
        }

        public object Create<TDestination, TSource>(TSource source, string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return Create<TDestination, TSource>(source);
            }
            else
            {
                var mappedSource = mapper.Map<TDestination>(source);
                return CreateDynamicObject(mappedSource, fields);
            }
        }

        private object CreateDynamicObject<TSource>(TSource source, string fields)
        {
            var fieldsList = fields.Split(',');

            ExpandoObject result = new ExpandoObject();
            foreach (var field in fieldsList)
            {
                PropertyInfo property = source.GetType()
                                              .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

                object value = property.GetValue(source);
                string propertyName = property.Name;

                ((IDictionary<string, object>)result).Add(propertyName, value);
            }

            return result;
        }

        public IEnumerable<dynamic> Create<TDestination, TSource>(IEnumerable<TSource> source, string fields)
        {
            var dtos = Create<TDestination, TSource>(source);

            if (string.IsNullOrWhiteSpace(fields))
            {
                return dtos.ToDynamicList();
            }

            var projections = new Projection(fields);

            return dtos.AsQueryable().Select(projections.ToProjectionString()).ToDynamicList();
        }
    }
}
