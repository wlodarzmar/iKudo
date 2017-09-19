using iKudo.Domain.Criteria;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace iKudo.Clients.Web.Controllers.Api.ModelBinders
{
    public class BoardSearchCriteriaBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(BoardSearchCriteria))
            {
                return new BoardSearchCriteriaBinder();
            }

            return null;
        }
    }
}
