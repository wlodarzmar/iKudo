using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using Microsoft.Extensions.Logging;
using Moq;

namespace iKudo.Clients.Web.Tests
{
    public abstract class KudoControllerTestsBase
    {
        public KudoControllerTestsBase()
        {
            KudoManagerMock = new Mock<IManageKudos>();
            DtoFactoryMock = new Mock<IDtoFactory>();
            LoggerMock = new Mock<ILogger<KudosController>>();
            Controller = new KudosController(DtoFactoryMock.Object, KudoManagerMock.Object, LoggerMock.Object);
            Controller.WithCurrentUser();
        }

        protected KudosController Controller { get; private set; }
        protected Mock<IManageKudos> KudoManagerMock { get; private set; }
        protected Mock<IDtoFactory> DtoFactoryMock { get; private set; }
        protected Mock<ILogger<KudosController>> LoggerMock { get; private set; }
    }
}
