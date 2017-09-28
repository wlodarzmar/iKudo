using iKudo.Domain.Criteria;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using iKudo.Parsers;

namespace iKudo.Clients.Web.Controllers.Api.ModelBinders
{
    public class KudosSearchCriteriaBinderProvider : IModelBinderProvider
    {
        private readonly IKudoSearchCriteriaParser parser;

        public KudosSearchCriteriaBinderProvider(IKudoSearchCriteriaParser searchCriteriaParser)
        {
            parser = searchCriteriaParser;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(KudosSearchCriteria))
            {
                return new KudosSearchCriteriaBinder(parser);
            }
            else
            {
                return null;
            }
        }
    }
}
