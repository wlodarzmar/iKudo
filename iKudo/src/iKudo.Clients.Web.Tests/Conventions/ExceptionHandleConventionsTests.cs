using FluentAssertions;
using iKudo.Clients.Web.Filters;
using iKudo.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Xunit;

namespace iKudo.Clients.Web.Tests.Conventions
{
    public class ExceptionHandleConventionsTests
    {
        [Fact]
        public void EveryControllerHasServiceFilterAttribute()
        {
            var apiControllers = typeof(BaseApiController).Assembly
                                    .GetTypes()
                                    .Where(x => x.BaseType == typeof(BaseApiController) || x.BaseType == typeof(Controller))
                                    .Where(x => !x.IsAbstract)
                                    .Where(x => x.Namespace.ToLower().Contains("api"));

            var attributes = apiControllers.SelectMany(x => x.CustomAttributes
                                           .Where(a => a.AttributeType == typeof(ServiceFilterAttribute))
                                           .Where(a => (a.ConstructorArguments.Single().Value as Type) == typeof(ExceptionHandle)));

            apiControllers.Should().NotBeEmpty();
            attributes.Count().Should().Be(apiControllers.Count());
        }
    }
}
