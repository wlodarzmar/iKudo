using FluentAssertions;
using iKudo.Clients.Web.Controllers.Api.ModelBinders;
using iKudo.Domain.Criteria;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Web;
using Xunit;

namespace iKudo.Clients.Web.Tests.BindersTests
{
    public class BoardSearchCriteriaBinderTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("\t")]
        public async void BindModel_ReturnsDefaultResult_WhenEmptyQueryString(string queryString)
        {
            DefaultModelBindingContext context = GetBindingContext();
            context.ValueProvider = GetValueProvider(queryString);
            var binder = new BoardSearchCriteriaBinder();

            await binder.BindModelAsync(context);

            context.Result.Model.As<BoardSearchCriteria>().Should().NotBeNull();
        }

        [Theory]
        [InlineData("Member name", "creator name")]
        public async void BindModel_ReturnsValidCriteria_WhenValidQueryString(string member, string creator)
        {
            DefaultModelBindingContext context = GetBindingContext();
            context.ValueProvider = GetValueProvider(member, creator); ciulowy query string powstaje
            var binder = new BoardSearchCriteriaBinder();

            await binder.BindModelAsync(context);

            BoardSearchCriteria criteria = context.Result.Model as BoardSearchCriteria;
            criteria.Member.Should().Be(member);
            criteria.CreatorId.Should().Be(creator);
        }

        private static DefaultModelBindingContext GetBindingContext()
        {
            var bindingContext = new DefaultModelBindingContext
            {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(BoardSearchCriteria)),
                ModelName = "theModelName",
                ModelState = new ModelStateDictionary(),
            };

            return bindingContext;
        }

        private static QueryStringValueProvider GetValueProvider(params string[] queryParts)
        {
            string queryString = $"?{string.Join('&', queryParts)}";
            return GetValueProvider(queryString);
        }

        private static QueryStringValueProvider GetValueProvider(string queryString)
        {
            Dictionary<string, StringValues> dict = new Dictionary<string, StringValues>();
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                var a = HttpUtility.ParseQueryString(queryString);
                List<KeyValuePair<string, StringValues>> asd = new List<KeyValuePair<string, StringValues>>();
                foreach (var key in a.Keys)
                {
                    asd.Add(new KeyValuePair<string, StringValues>(key.ToString(), a[key.ToString()].ToString()));
                }
                dict = new Dictionary<string, StringValues>(asd);
            }

            QueryCollection queryCollection = new QueryCollection(dict);
            BindingSource bindingSource = new BindingSource(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false, false);
            QueryStringValueProvider valueProvider = new QueryStringValueProvider(bindingSource, queryCollection, System.Globalization.CultureInfo.CurrentCulture);

            return valueProvider;
        }
    }
}
