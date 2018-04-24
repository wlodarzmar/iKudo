using iKudo.Clients.Web.Filters;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace iKudo.Controllers.Api
{
    [ServiceFilter(typeof(ExceptionHandleAttribute))]
    public class NotificationsController : BaseApiController
    {
        private readonly INotify notifier;
        private readonly IDtoFactory dtoFactory;

        public NotificationsController(INotify notifier, IDtoFactory dtoFactory, ILogger<NotificationsController> logger)
            : base(logger)
        {
            this.notifier = notifier;
            this.dtoFactory = dtoFactory;
        }

        [HttpPut, Authorize]
        [ValidationFilterAttribute]
        [Route("api/notifications")]
        public IActionResult Put([FromBody] NotificationDto notificationDto)
        {
            try
            {
                Notification notification = dtoFactory.Create<Notification, NotificationDto>(notificationDto);
                string userId = CurrentUserId;
                notifier.Update(userId, notification);

                Logger.LogInformation("User {user} updated notification: {@notification}", CurrentUserId, notification);

                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Forbidden);
            }
        }

        [HttpGet, Authorize]
        [Route("api/notifications")]
        public IActionResult Get(NotificationSearchCriteria searchCriteria)
        {
            SortCriteria sortCriteria = new SortCriteria { RawCriteria = searchCriteria.Sort };

            IEnumerable<NotificationMessage> notifications = notifier.Get(searchCriteria, sortCriteria);
            IEnumerable<NotificationDto> notificationDtos = dtoFactory.Create<NotificationDto, NotificationMessage>(notifications);
            return Ok(notificationDtos);
        }
    }
}
