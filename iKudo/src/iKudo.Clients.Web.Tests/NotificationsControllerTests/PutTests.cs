using FluentAssertions;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests.NotificationsControllerTests
{
    public class PutTests : NotificationControllerTestBase
    {
        [Fact]
        public void Put_ValidRequest_ReturnsOk()
        {
            NotificationDTO notificationDto = new NotificationDTO { };

            OkResult response = Controller.Put(notificationDto) as OkResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void Put_ValidRequest_CallsDtoFactoryWithValidNotification()
        {
            NotificationDTO notificationDto = new NotificationDTO { Id = 1 };

            Controller.Put(notificationDto);

            DtoFactoryMock.Verify(x => x.Create<Notification, NotificationDTO>(It.Is<NotificationDTO>(n => n.Id == 1)));
        }

        [Fact]
        public void Put_ValidRequest_CallsNotifierUpdateWithValidNotification()
        {
            DtoFactoryMock.Setup(x => x.Create<Notification, NotificationDTO>(It.IsAny<NotificationDTO>())).Returns(new Notification() { Id = 1 });
            NotificationDTO notificationDto = new NotificationDTO { Id = 1, ReceiverId = "receiver", SenderId = "sender" };

            Controller.Put(notificationDto);

            NotifierMock.Verify(x => x.Update(It.IsAny<string>(), It.Is<Notification>(n => n.Id == 1)));
        }

        [Fact]
        public void Put_NotifierThrowsUnauthorizedAccessException_ReturnsForbidden()
        {
            NotifierMock.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Notification>())).Throws(new UnauthorizedAccessException());
            NotificationDTO notificationDto = new NotificationDTO { Id = 1, ReceiverId = "receiver", SenderId = "sender" };

            StatusCodeResult response = Controller.Put(notificationDto) as StatusCodeResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
        }
    }
}
