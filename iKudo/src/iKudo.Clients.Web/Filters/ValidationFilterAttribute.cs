using iKudo.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;

namespace iKudo.Clients.Web.Filters
{
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                string[] errors = context.ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage)).ToArray();
                string error = string.Join(',', errors);
                context.Result = new BadRequestObjectResult(new ErrorResult(error, HttpStatusCode.BadRequest));
            }
        }
    }
}
