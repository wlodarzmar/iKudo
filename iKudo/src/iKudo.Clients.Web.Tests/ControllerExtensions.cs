using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace iKudo.Clients.Web.Tests
{
    public static class ControllerExtensions
    {
        public static void WithCurrentUser(this Controller controller, string userId)
        {
            AssignControllerContext(controller, userId);
        }

        public static void WithCurrentUser(this Controller controller)
        {
            AssignControllerContext(controller, null);
        }

        private static void AssignControllerContext(Controller controller, string userId)
        {
            List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId ?? "") };
            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }
    }
}
