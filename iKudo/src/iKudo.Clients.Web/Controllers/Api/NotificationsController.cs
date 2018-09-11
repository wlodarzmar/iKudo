using iKudo.Clients.Web.Dtos.Notifications;
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
        private readonly IManageNotifications notificationManager;
        private readonly IProvideNotifications notificationsProvider;
        private readonly IDtoFactory dtoFactory;

        public NotificationsController(
            IManageNotifications notificationManager,
            IProvideNotifications notificationsProvider,
            IDtoFactory dtoFactory,
            ILogger<NotificationsController> logger)
            : base(logger)
        {
            this.notificationManager = notificationManager;
            this.notificationsProvider = notificationsProvider;
            this.dtoFactory = dtoFactory;
        }

        [HttpPut, Authorize]
        [ValidationFilter]
        [Route("api/notifications")]
        public IActionResult Put([FromBody] NotificationDto notificationDto)
        {
            try
            {
                Notification notification = dtoFactory.Create<Notification, NotificationDto>(notificationDto);
                string userId = CurrentUserId;
                notificationManager.Update(userId, notification);

                Logger.LogInformation("User {user} updated notification: {@notification}", CurrentUserId, notification);

                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError(BUSSINESS_ERROR_MESSAGE_TEMPLATE, ex);
                return StatusCode((int)HttpStatusCode.Forbidden);
            }
        }

        [Authorize, HttpGet]
        [Route("api/notifications")]
        public IActionResult Get(NotificationGetParameters parameters)
        {
            var searchCriteria = new NotificationSearchCriteria
            {
                IsRead = parameters.IsRead,
                Receiver = CurrentUserId
            };
            var sortCriteria = new SortCriteria { RawCriteria = parameters.Sort };

            IEnumerable<Notification> notifications = notificationsProvider.Get(searchCriteria, sortCriteria);
            var notificationsDtos = dtoFactory.Create<NotificationDto, Notification>(notifications, parameters.Fields);
            return Ok(notificationsDtos);
        }
    }
}