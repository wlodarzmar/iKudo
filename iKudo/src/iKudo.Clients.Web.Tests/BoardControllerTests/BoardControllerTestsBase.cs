using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using Microsoft.Extensions.Logging;
using Moq;

namespace iKudo.Clients.Web.Tests
{
    public abstract class BoardControllerTestsBase
    {
        protected BoardControllerTestsBase()
        {
            BoardManagerMock = new Mock<IManageBoards>();
            BoardProviderMock = new Mock<IProvideBoards>();
            DtoFactoryMock = new Mock<IDtoFactory>();
            LoggerMock = new Mock<ILogger<BoardController>>();

            Controller = new BoardController(BoardManagerMock.Object, BoardProviderMock.Object, DtoFactoryMock.Object, LoggerMock.Object);
            Controller.WithCurrentUser();
        }

        protected BoardController Controller { get; private set; }
        protected Mock<IManageBoards> BoardManagerMock { get; private set; }
        protected Mock<IProvideBoards> BoardProviderMock { get; private set; }
        protected Mock<IDtoFactory> DtoFactoryMock { get; private set; }
        protected Mock<ILogger<BoardController>> LoggerMock { get; private set; }
    }
}
