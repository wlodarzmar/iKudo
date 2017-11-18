using iKudo.Controllers.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace iKudo.Clients.Web.Filters
{
    public class ExceptionHandle : ExceptionFilterAttribute
    {
        private ILogger<ExceptionHandle> logger;

        public ExceptionHandle(ILogger<ExceptionHandle> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogCritical("Exception occurred: {@exception}", context.Exception);

            var result = new ObjectResult(new ErrorResult("Internal error"))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.Result = result;
        }
    }
}
