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
            NotificationDto notificationDto = new NotificationDto { };

            OkResult response = Controller.Put(notificationDto) as OkResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void Put_ValidRequest_CallsDtoFactoryWithValidNotification()
        {
            NotificationDto notificationDto = new NotificationDto { Id = 1 };

            Controller.Put(notificationDto);

            DtoFactoryMock.Verify(x => x.Create<Notification, NotificationDto>(It.Is<NotificationDto>(n => n.Id == 1)));
        }

        [Fact]
        public void Put_ValidRequest_CallsNotifierUpdateWithValidNotification()
        {
            DtoFactoryMock.Setup(x => x.Create<Notification, NotificationDto>(It.IsAny<NotificationDto>())).Returns(new Notification() { Id = 1 });
            NotificationDto notificationDto = new NotificationDto { Id = 1, ReceiverId = "receiver", SenderId = "sender" };

            Controller.Put(notificationDto);

            NotificationManagerMock.Verify(x => x.Update(It.IsAny<string>(), It.Is<Notification>(n => n.Id == 1)));
        }

        [Fact]
        public void Put_NotifierThrowsUnauthorizedAccessException_ReturnsForbidden()
        {
            NotificationManagerMock.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Notification>())).Throws(new UnauthorizedAccessException());
            NotificationDto notificationDto = new NotificationDto { Id = 1, ReceiverId = "receiver", SenderId = "sender" };

            StatusCodeResult response = Controller.Put(notificationDto) as StatusCodeResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
        }
    }
}
