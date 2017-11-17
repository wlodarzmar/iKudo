using iKudo.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace iKudo.Clients.Web.Filters
{
    public class ExceptionHandle : ExceptionFilterAttribute
    {
        //private readonly ILogger<ExceptionHandle> logger;

        public ExceptionHandle()
        {

        }

        public override void OnException(ExceptionContext context)
        {
            var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger)) as ILogger;

            if (context.Exception is KudoException)
            {
                logger.LogError("Business exception occurred", context.Exception);
            }
            else
            {
                logger.LogCritical("Exception occurred", context.Exception);
            }
        }
    }
}
