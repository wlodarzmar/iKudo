using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
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

        [Fact]
        public void Get_WithReceiver_CallsNotifierWithReceiverParameter()
        {
            NotificationsController controller = new NotificationsController(notifierMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser("currentUser");

            controller.Get("receiver");

            notifierMock.Verify(x => x.Get(It.Is<NotificationSearchCriteria>(c => c.Receiver == "receiver"),It.IsAny<SortCriteria>()), Times.Once);
        }

        [Fact]
        public void Get_WithIsRead_CallsNotifierWithIsReadParameter()
        {
            NotificationsController controller = new NotificationsController(notifierMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser("currentUser");

            controller.Get(isRead: true);

            notifierMock.Verify(x => x.Get(It.Is<NotificationSearchCriteria>(c => c.IsRead == true), It.IsAny<SortCriteria>()), Times.Once);
        }

        [Fact]
        public void Get_NotifierGetReturnsNotifications_dtoFactoryIsCalledWithValidCollection()
        {
            IEnumerable<NotificationMessage> notifications = new List<NotificationMessage> {
                new NotificationMessage( new Notification("sender", "receiver", DateTime.Now, NotificationTypes.BoardJoinAccepted))
            };
            notifierMock.Setup(x => x.Get(It.IsAny<NotificationSearchCriteria>(), It.IsAny<SortCriteria>())).Returns(notifications);
            NotificationsController controller = new NotificationsController(notifierMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser("currentUser");

            controller.Get("receiver", false);

            dtoFactoryMock.Verify(x => x.Create<NotificationDTO, NotificationMessage>(It.Is<IEnumerable<NotificationMessage>>(s => s.Count() == notifications.Count())));
        }

        [Fact]
        public void Get_WithSort_CallsNotifierWithSortingParameters()
        {
            NotificationsController controller = new NotificationsController(notifierMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser("currentUser");

            controller.Get(sort: "creationDate");

            notifierMock.Verify(x => x.Get(It.IsAny<NotificationSearchCriteria>(), It.Is<SortCriteria>(c=>c.SortCriteriaText == "creationDate")));
        }
    }
}
