using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using Moq;

namespace iKudo.Domain.Tests
{
    public class BoardManagerBaseTest : BaseTest
    {
        public BoardManagerBaseTest()
        {
            FileStorageMock = new Mock<IFileStorage>();
            Manager = new BoardManager(DbContext, TimeProviderMock.Object, FileStorageMock.Object);
        }

        public IManageBoards Manager { get; set; }

        public Mock<IFileStorage> FileStorageMock { get; set; }
    }
}
