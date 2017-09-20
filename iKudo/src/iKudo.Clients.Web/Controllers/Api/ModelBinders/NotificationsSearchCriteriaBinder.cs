using iKudo.Common;
using iKudo.Domain.Criteria;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace iKudo.Clients.Web.Controllers.Api.ModelBinders
{
    public class NotificationsSearchCriteriaBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string receiver = bindingContext.ValueProvider.GetValue("receiver").FirstValue;
            bool? isRead = bindingContext.ValueProvider.GetValue("isRead").FirstValue?.ToNullableBool();
            string sort = bindingContext.ValueProvider.GetValue("sort").FirstValue;

            NotificationSearchCriteria criteria = new NotificationSearchCriteria
            {
                IsRead = isRead,
                Receiver = receiver,
                Sort = sort
            };

            bindingContext.Result = ModelBindingResult.Success(criteria);

            return Task.CompletedTask;
        }
    }
}
