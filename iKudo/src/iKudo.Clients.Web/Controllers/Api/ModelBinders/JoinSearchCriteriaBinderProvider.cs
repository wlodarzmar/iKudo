using iKudo.Domain.Criteria;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace iKudo.Clients.Web.Controllers.Api.ModelBinders
{
    public class JoinSearchCriteriaBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(JoinSearchCriteria))
            {
                return new JoinSearchCriteriaBinder();
            }
            else
            {
                return null;
            }
        }
    }
}
