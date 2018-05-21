using iKudo.Common;
using iKudo.Domain.Criteria;
using iKudo.Parsers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace iKudo.Clients.Web.Controllers.Api.ModelBinders
{
    public class KudosSearchCriteriaBinder : IModelBinder
    {
        private readonly IKudoSearchCriteriaParser parser;

        public KudosSearchCriteriaBinder(IKudoSearchCriteriaParser parser)
        {
            this.parser = parser;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            int? boardId = bindingContext.ValueProvider.GetValue("boardId").FirstValue?.ToNullableInt();
            string sender = bindingContext.ValueProvider.GetValue("sender").FirstValue;
            string receiver = bindingContext.ValueProvider.GetValue("receiver").FirstValue;
            string senderOrReceiver = bindingContext.ValueProvider.GetValue("user").FirstValue;
            string status = bindingContext.ValueProvider.GetValue("status").FirstValue;
            string sort = bindingContext.ValueProvider.GetValue("sort").FirstValue;

            string currentUser = bindingContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            KudosSearchCriteria criteria = parser.Parse(currentUser, boardId, sender, receiver, senderOrReceiver, status)
                                                 .WithSort(sort);

            bindingContext.Result = ModelBindingResult.Success(criteria);

            return Task.CompletedTask;
        }
    }
}
