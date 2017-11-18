using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using Moq;

namespace iKudo.Clients.Web.Tests
{
    public abstract class JoinRequestControllerTestsBase
    {
        public JoinRequestControllerTestsBase()
        {
            DtoFactoryMock = new Mock<IDtoFactory>();
            JoinManagerMock = new Mock<IManageJoins>();
            Controller = new JoinRequestController(JoinManagerMock.Object, DtoFactoryMock.Object);
            Controller.WithCurrentUser();
        }

        protected Mock<IDtoFactory> DtoFactoryMock { get; private set; }
        protected Mock<IManageJoins> JoinManagerMock { get; private set; }
        protected JoinRequestController Controller { get; private set; }
    }
}
