using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace iKudo.Clients.Web.Tests
{
    public class BoardControllerTestBase
    {
        protected ControllerContext GetControllerContext(string userId = null)
        {
            List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId ?? "") };
            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }
    }
}