using iKudo.Clients.Web.Dtos.Notifications;
using iKudo.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace iKudo.Clients.Web.Controllers.Api.ModelBinders
{
    public class NotificationGetParametersModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string receiver = bindingContext.ValueProvider.GetValue("receiver").FirstValue;
            bool? isRead = bindingContext.ValueProvider.GetValue("isRead").FirstValue?.ToNullableBool();
            string sort = bindingContext.ValueProvider.GetValue("sort").FirstValue;

            var parameters = new NotificationGetParameters
            {
                IsRead = isRead,
                Receiver = receiver,
                Sort = sort
            };

            bindingContext.Result = ModelBindingResult.Success(parameters);

            return Task.CompletedTask;
        }
    }
}
