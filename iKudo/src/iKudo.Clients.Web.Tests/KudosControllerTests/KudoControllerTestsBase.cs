using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using Moq;

namespace iKudo.Clients.Web.Tests
{
    public abstract class KudoControllerTestsBase
    {
        public KudoControllerTestsBase()
        {
            KudoManagerMock = new Mock<IManageKudos>();
            DtoFactoryMock = new Mock<IDtoFactory>();
            Controller = new KudosController(DtoFactoryMock.Object, KudoManagerMock.Object);
            Controller.WithCurrentUser();
        }

        protected KudosController Controller { get; private set; }
        protected Mock<IManageKudos> KudoManagerMock { get; private set; }
        protected Mock<IDtoFactory> DtoFactoryMock { get; private set; }
    }
}
