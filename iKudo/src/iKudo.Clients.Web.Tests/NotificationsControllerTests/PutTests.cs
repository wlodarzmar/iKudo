using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using System.Net;
using iKudo.Domain.Model;
using System;

namespace iKudo.Clients.Web.Tests.NotificationsControllerTests
{
    public class PutTests
    {
        private Mock<INotify> notifierMock;
        private Mock<IDtoFactory> dtoFactoryMock;

        public PutTests()
        {
            notifierMock = new Mock<INotify>();
            dtoFactoryMock = new Mock<IDtoFactory>();
        }

        [Fact]
        public void Put_ValidRequest_ReturnsOk()
        {
            NotificationsController controller = new NotificationsController(notifierMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            NotificationDTO notificationDto = new NotificationDTO { };
            OkResult response = controller.Put(notificationDto) as OkResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void Put_ValidRequest_CallsDtoFactoryWithValidNotification()
        {
            NotificationsController controller = new NotificationsController(notifierMock.Object, dtoFactoryMock.Object);

            NotificationDTO notificationDto = new NotificationDTO { Id = 1 };
            controller.Put(notificationDto);

            dtoFactoryMock.Verify(x => x.Create<Notification, NotificationDTO>(It.Is<NotificationDTO>(n => n.Id == 1)));
        }

        [Fact]
        public void Put_ValidRequest_CallsNotifierUpdateWithValidNotification()
        {
            dtoFactoryMock.Setup(x => x.Create<Notification, NotificationDTO>(It.IsAny<NotificationDTO>())).Returns(new Notification() { Id = 1 });
            NotificationsController controller = new NotificationsController(notifierMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            NotificationDTO notificationDto = new NotificationDTO { Id = 1, ReceiverId = "receiver", SenderId = "sender" };
            controller.Put(notificationDto);

            notifierMock.Verify(x => x.Update(It.IsAny<string>(), It.Is<Notification>(n => n.Id == 1)));
        }

        [Fact]
        public void Put_UnknownExceptionThrown_ReturnsInternalServerError()
        {
            notifierMock.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Notification>())).Throws(new Exception());
            NotificationsController controller = new NotificationsController(notifierMock.Object, dtoFactoryMock.Object);

            NotificationDTO notificationDto = new NotificationDTO { Id = 1, ReceiverId = "receiver", SenderId = "sender" };
            ObjectResult response = controller.Put(notificationDto) as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrWhiteSpace();
        }
        
        [Fact]
        public void Put_NotifierThrowsUnauthorizedAccessException_ReturnsForbidden()
        {
            notifierMock.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<Notification>())).Throws(new UnauthorizedAccessException());
            NotificationsController controller = new NotificationsController(notifierMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            NotificationDTO notificationDto = new NotificationDTO { Id = 1, ReceiverId = "receiver", SenderId = "sender" };
            StatusCodeResult response = controller.Put(notificationDto) as StatusCodeResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
        }
    }
}
