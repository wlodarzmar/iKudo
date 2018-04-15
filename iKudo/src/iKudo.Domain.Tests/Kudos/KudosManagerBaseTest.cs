using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Moq;

namespace iKudo.Domain.Tests.Kudos
{
    public class KudosManagerBaseTest : BaseTest
    {
        public KudosManagerBaseTest()
        {
            FileStorageMock = new Mock<IFileStorage>();
            Manager = new KudosManager(DbContext, TimeProviderMock.Object, FileStorageMock.Object);
        }

        public IManageKudos Manager { get; set; }

        public Mock<IFileStorage> FileStorageMock { get; set; }

        protected Kudo CreateKudo(int boardId)
        {
            return CreateKudo(boardId, "", "", false);
        }

        protected Kudo CreateKudo(int boardId, string boardCreator, string senderId, bool isAnonymous)
        {
            return CreateKudo(boardId, boardCreator, senderId, string.Empty, isAnonymous);
        }

        protected Kudo CreateKudo(int boardId, string boardCreator, string senderId, string receiverId, bool isAnonymous)
        {
            return new Kudo
            {
                BoardId = boardId,
                Board = new Board { Id = boardId, CreatorId = boardCreator },
                SenderId = senderId,
                Sender = new User { Id = senderId },
                ReceiverId = receiverId,
                Receiver = new User { Id = receiverId },
                IsAnonymous = isAnonymous
            };
        }

        protected Kudo CreateKudo(Board board, string senderId, string receiverId, bool isAnonymous)
        {
            return new Kudo
            {
                Board = board,
                BoardId = board.Id,
                SenderId = senderId,
                Sender = new User { Id = senderId },
                ReceiverId = receiverId,
                Receiver = new User { Id = receiverId },
                IsAnonymous = isAnonymous
            };
        }
    }
}
