using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests.NotificationsControllerTests
{
    public class CountTests
    {
        private Mock<INotify> notifierMock;

        public CountTests()
        {
            notifierMock = new Mock<INotify>();
        }

        [Fact]
        public void Count_WithoutAnyNotifications_ReturnsZero()
        {
            NotificationsController controller = new NotificationsController(notifierMock.Object);
            controller.WithCurrentUser("receiver");

            OkObjectResult response = controller.Count() as OkObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            response.Value.As<int>().Should().Be(0);
        }

        [Fact]
        public void Count_NotifierReturnsNotificationCount_ReturnsNotificationCount()
        {
            string receiverId = "receiver";
            notifierMock.Setup(x => x.Count(It.Is<string>(p => p == receiverId))).Returns(55);
            NotificationsController controller = new NotificationsController(notifierMock.Object);
            controller.WithCurrentUser(receiverId);
            OkObjectResult response = controller.Count() as OkObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            response.Value.As<int>().Should().Be(55);
        }

        [Fact]
        public void Count_WhenUnknownExceptionThrown_ReturnsInternalServerError()
        {
            notifierMock.Setup(x => x.Count(It.IsAny<string>())).Throws(new System.Exception());
            NotificationsController controller = new NotificationsController(notifierMock.Object);
            controller.WithCurrentUser("receiver");

            ObjectResult response = controller.Count() as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrWhiteSpace();
        }
    }
}
