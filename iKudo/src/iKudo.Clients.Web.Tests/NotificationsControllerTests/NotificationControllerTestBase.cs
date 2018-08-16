using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using Microsoft.Extensions.Logging;
using Moq;

namespace iKudo.Clients.Web.Tests.NotificationsControllerTests
{
    public abstract class NotificationControllerTestBase
    {
        protected NotificationControllerTestBase()
        {
            NotifierMock = new Mock<IManageNotifications>();
            DtoFactoryMock = new Mock<IDtoFactory>();
            LoggerMock = new Mock<ILogger<NotificationsController>>();
            Controller = new NotificationsController(NotifierMock.Object, DtoFactoryMock.Object, LoggerMock.Object);
            Controller.WithCurrentUser();
        }

        protected NotificationsController Controller { get; private set; }
        protected Mock<IManageNotifications> NotifierMock { get; private set; }
        protected Mock<IDtoFactory> DtoFactoryMock { get; private set; }
        protected Mock<ILogger<NotificationsController>> LoggerMock { get; private set; }
    }
}
