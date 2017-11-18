using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using Moq;

namespace iKudo.Clients.Web.Tests.NotificationsControllerTests
{
    public abstract class NotificationControllerTestBase
    {
        public NotificationControllerTestBase()
        {
            NotifierMock = new Mock<INotify>();
            DtoFactoryMock = new Mock<IDtoFactory>();
            Controller = new NotificationsController(NotifierMock.Object, DtoFactoryMock.Object);
            Controller.WithCurrentUser();
        }

        protected NotificationsController Controller { get; private set; }
        protected Mock<INotify> NotifierMock { get; private set; }
        protected Mock<IDtoFactory> DtoFactoryMock { get; private set; }
    }
}
