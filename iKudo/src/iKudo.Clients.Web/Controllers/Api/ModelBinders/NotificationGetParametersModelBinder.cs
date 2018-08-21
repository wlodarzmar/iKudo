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
            bool? isRead = bindingContext.ValueProvider.GetValue("isRead").FirstValue?.ToNullableBool();
            string sort = bindingContext.ValueProvider.GetValue("sort").FirstValue;
            string fields = bindingContext.ValueProvider.GetValue("fields").FirstValue;

            var parameters = new NotificationGetParameters
            {
                IsRead = isRead,
                Sort = sort,
                Fields = fields
            };

            bindingContext.Result = ModelBindingResult.Success(parameters);

            return Task.CompletedTask;
        }
    }
}
