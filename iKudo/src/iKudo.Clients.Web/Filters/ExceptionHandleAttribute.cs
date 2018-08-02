using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace iKudo.Clients.Web.Filters
{
    public class ExceptionHandleAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionHandleAttribute> logger;

        public ExceptionHandleAttribute(ILogger<ExceptionHandleAttribute> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            string errorMessage = "Internal error";
            if (context.Exception is KudoException)
            {
                errorMessage = context.Exception.Message;
            }

            logger.LogCritical("Exception occurred: {@exception}", context.Exception);

            var result = new ObjectResult(new ErrorResult(errorMessage, StatusCodes.Status500InternalServerError))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.Result = result;
        }
    }
}
