using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace iKudo.Controllers.Api
{
    public abstract class BaseApiController : Controller
    {
        protected const string BUSSINESS_ERROR_MESSAGE_TEMPLATE = "Business error: {@exception}";

        protected BaseApiController(ILogger<BaseApiController> logger)
        {
            Logger = logger;
        }

        protected string CurrentUserId => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        public ILogger<BaseApiController> Logger { get; private set; }
    }
}
