using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace iKudo.Controllers.Api
{
    public class NotificationsController : Controller
    {
        [HttpGet]
        [Route("api/notifications/total")]
        public IActionResult Count()
        {
            return Ok(3);

            //try
            //{
            //    throw new NotImplementedException();
            //}
            //catch (Exception)
            //{
            //    return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult("Something went wrong"));
            //}
        }
    }
}
