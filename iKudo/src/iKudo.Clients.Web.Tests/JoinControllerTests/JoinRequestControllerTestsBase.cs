using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using Microsoft.Extensions.Logging;
using Moq;

namespace iKudo.Clients.Web.Tests
{
    public abstract class JoinRequestControllerTestsBase
    {
        protected JoinRequestControllerTestsBase()
        {
            DtoFactoryMock = new Mock<IDtoFactory>();
            JoinManagerMock = new Mock<IManageJoins>();
            LoggerMock = new Mock<ILogger<JoinRequestController>>();
            Controller = new JoinRequestController(JoinManagerMock.Object, DtoFactoryMock.Object, LoggerMock.Object);
            Controller.WithCurrentUser();
        }

        protected JoinRequestController Controller { get; private set; }
        protected Mock<IDtoFactory> DtoFactoryMock { get; private set; }
        protected Mock<IManageJoins> JoinManagerMock { get; private set; }
        protected Mock<ILogger<JoinRequestController>> LoggerMock { get; private set; }
    }
}
