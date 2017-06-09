using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using iKudo.Domain.Interfaces;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using iKudo.Domain.Logic;
using iKudo.Dtos;
using System.Collections.Generic;
using iKudo.Domain.Model;

namespace iKudo.Controllers.Api
{
    [Route("api/notifications")]
    public class NotificationsController : Controller
    {
        private INotify notifier;
        private IDtoFactory dtoFactory;

        public NotificationsController(INotify notifier, IDtoFactory dtoFactory)
        {
            this.notifier = notifier;
            this.dtoFactory = dtoFactory;
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

        [HttpGet, Authorize]
        public IActionResult Get(string receiver, bool isRead)
        {
            try
            {
                IEnumerable<NotificationDTO> notificationDtos = dtoFactory.Create<NotificationDTO, Notification>(new List<Notification> { });
                return Ok(notificationDtos);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResult("Something went wrong"));
            }
        }
    }
}
