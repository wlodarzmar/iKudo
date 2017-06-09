using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using iKudo.Domain.Interfaces;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace iKudo.Controllers.Api
{
    public class NotificationsController : Controller
    {
        private INotify notifier;

        public NotificationsController(INotify notifier)
        {
            this.notifier = notifier;
        }

        [HttpGet, Authorize]
        [Route("api/notifications/total")]
        public IActionResult Count()
        {
            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
                int noticationCount = notifier.Count(userId);
                return Ok(noticationCount);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult("Something went wrong"));
            }
        }
    }
}
