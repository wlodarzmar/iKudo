using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using iKudo.Domain.Interfaces;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using iKudo.Dtos;
using System.Collections.Generic;
using iKudo.Domain.Model;
using iKudo.Domain.Criteria;
using iKudo.Clients.Web.Filters;

namespace iKudo.Controllers.Api
{
    [ServiceFilter(typeof(ExceptionHandle))]
    public class NotificationsController : BaseApiController
    {
        private INotify notifier;
        private IDtoFactory dtoFactory;

        public NotificationsController(INotify notifier, IDtoFactory dtoFactory)
        {
            this.notifier = notifier;
            this.dtoFactory = dtoFactory;
        }

        [HttpPut, Authorize]
        [ValidationFilter]
        [Route("api/notifications")]
        public IActionResult Put([FromBody] NotificationDTO notificationDto)
        {
            try
            {
                Notification notification = dtoFactory.Create<Notification, NotificationDTO>(notificationDto);
                string userId = CurrentUserId;
                notifier.Update(userId, notification);

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode((int)HttpStatusCode.Forbidden);
            }
        }

        [HttpGet, Authorize]
        [Route("api/notifications")]
        public IActionResult Get(NotificationSearchCriteria searchCriteria)
        {
            SortCriteria sortCriteria = new SortCriteria { RawCriteria = searchCriteria.Sort };

            IEnumerable<NotificationMessage> notifications = notifier.Get(searchCriteria, sortCriteria);
            IEnumerable<NotificationDTO> notificationDtos = dtoFactory.Create<NotificationDTO, NotificationMessage>(notifications);
            return Ok(notificationDtos);
        }
    }
}
