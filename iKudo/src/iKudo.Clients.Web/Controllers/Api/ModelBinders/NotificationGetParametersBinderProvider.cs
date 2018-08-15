using iKudo.Clients.Web.Dtos.Notifications;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace iKudo.Clients.Web.Controllers.Api.ModelBinders
{
    public class NotificationGetParametersBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(NotificationGetParameters))
            {
                return new NotificationGetParametersModelBinder();
            }
            else
            {
                return null;
            }
        }
    }
}
