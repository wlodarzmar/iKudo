using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace iKudo.Clients.Web.Tests.NotificationsControllerTests
{
    public class GetTests
    {
        private Mock<INotify> notifierMock;
        private Mock<IDtoFactory> dtoFactoryMock;

        public GetTests()
        {
            notifierMock = new Mock<INotify>();
            dtoFactoryMock = new Mock<IDtoFactory>();
        }

        [Fact]
        public void Get_WithNotifications_ReturnsOkObjectResult()
        {
            IEnumerable<NotificationDTO> notifications = new List<NotificationDTO> {
                new NotificationDTO {ReceiverId = "user", ReadDate = DateTime.Now  }
            };
            dtoFactoryMock.Setup(x => x.Create<NotificationDTO, Notification>(It.IsAny<IEnumerable<Notification>>())).Returns(notifications);
            NotificationsController controller = new NotificationsController(notifierMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser("user");

            OkObjectResult response = controller.Get(receiver: "user", isRead: false) as OkObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            response.Value.As<IEnumerable<NotificationDTO>>().Count().Should().Be(1);
        }

        [Fact]
        public void Get_UnknownExceptionThrown_ReturnsInternalServerError()
        {
            dtoFactoryMock.Setup(x => x.Create<NotificationDTO, Notification>(It.IsAny<IEnumerable<Notification>>()))
                          .Throws(new Exception());

            NotificationsController controller = new NotificationsController(notifierMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser("user");

            ObjectResult response = controller.Get(receiver: "user", isRead: false) as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
