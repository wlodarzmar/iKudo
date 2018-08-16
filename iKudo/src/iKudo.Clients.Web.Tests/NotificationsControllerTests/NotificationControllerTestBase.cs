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
            NotificationManagerMock = new Mock<IManageNotifications>();
            NotificationsProviderMock = new Mock<IProvideNotifications>();
            DtoFactoryMock = new Mock<IDtoFactory>();
            LoggerMock = new Mock<ILogger<NotificationsController>>();
            Controller = new NotificationsController(NotificationManagerMock.Object, NotificationsProviderMock.Object, DtoFactoryMock.Object, LoggerMock.Object);
            Controller.WithCurrentUser();
        }

        protected NotificationsController Controller { get; private set; }
        protected Mock<IManageNotifications> NotificationManagerMock { get; private set; }
        protected Mock<IProvideNotifications> NotificationsProviderMock { get; private set; }
        protected Mock<IDtoFactory> DtoFactoryMock { get; private set; }
        protected Mock<ILogger<NotificationsController>> LoggerMock { get; private set; }
    }
}
