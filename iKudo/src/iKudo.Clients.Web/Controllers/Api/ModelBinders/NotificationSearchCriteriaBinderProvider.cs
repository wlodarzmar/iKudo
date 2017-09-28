using iKudo.Domain.Criteria;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace iKudo.Clients.Web.Controllers.Api.ModelBinders
{
    public class NotificationSearchCriteriaBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(NotificationSearchCriteria))
            {
                return new NotificationsSearchCriteriaBinder();
            }
            else
            {
                return null;
            }
        }
    }
}
