using iKudo.Domain.Criteria;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace iKudo.Clients.Web.Controllers.Api.ModelBinders
{
    public class BoardSearchCriteriaBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string creator = bindingContext.ValueProvider.GetValue("creator").FirstValue;
            string member = bindingContext.ValueProvider.GetValue("member").FirstValue;
            
            BoardSearchCriteria criteria = new BoardSearchCriteria
            {
                CreatorId = creator,
                Member = member
            };

            bindingContext.Result = ModelBindingResult.Success(criteria);

            return Task.CompletedTask;
        }
    }
}
