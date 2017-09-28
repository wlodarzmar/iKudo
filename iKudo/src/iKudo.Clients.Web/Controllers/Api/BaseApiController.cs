using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace iKudo.Controllers.Api
{
    public class BaseApiController : Controller
    {
        public string CurrentUserId => User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
    }
}
