using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using iKudo.Domain.Criteria;
using iKudo.Common;

namespace iKudo.Clients.Web.Controllers.Api.ModelBinders
{
    public class JoinSearchCriteriaBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            int? boardId = bindingContext.ValueProvider.GetValue("boardId").FirstValue?.ToNullableInt();
            string status = bindingContext.ValueProvider.GetValue("status").FirstValue;
            string candidateId = bindingContext.ValueProvider.GetValue("candidateId").FirstValue;
            
            JoinSearchCriteria criteria = new JoinSearchCriteria
            {
                BoardId = boardId,
                StatusText = status,
                CandidateId = candidateId
            };

            bindingContext.Result = ModelBindingResult.Success(criteria);

            return Task.CompletedTask;
        }
    }
}
