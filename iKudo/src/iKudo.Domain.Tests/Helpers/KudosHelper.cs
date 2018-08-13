using iKudo.Domain.Enums;
using iKudo.Domain.Model;

namespace iKudo.Domain.Tests.Helpers
{
    public static class KudosHelper
    {
        public static Kudo CreateKudo(int boardId)
        {
            return CreateKudo(boardId, "boardCreator", "senderId", false);
        }

        public static Kudo CreateKudo(int boardId, string boardCreator, string senderId, bool isAnonymous)
        {
            return CreateKudo(boardId, boardCreator, senderId, string.Empty, isAnonymous);
        }

        public static Kudo CreateKudo(int boardId, string boardCreator, string senderId, string receiverId, bool isAnonymous)
        {
            return new Kudo
            {
                BoardId = boardId,
                Board = new Board { Id = boardId, CreatorId = boardCreator },
                SenderId = senderId,
                Sender = new User { Id = senderId },
                ReceiverId = receiverId,
                Receiver = new User { Id = receiverId },
                IsAnonymous = isAnonymous,
                Status = KudoStatus.Accepted
            };
        }

        public static Kudo CreateKudo(Board board, string senderId, string receiverId, bool isAnonymous)
        {
            return CreateKudo(board, senderId, receiverId, isAnonymous, string.Empty);
        }

        public static Kudo CreateKudo(Board board, string senderId, string receiverId, bool isAnonymous, string description)
        {
            return new Kudo
            {
                Board = board,
                BoardId = board.Id,
                SenderId = senderId,
                Sender = new User { Id = senderId },
                ReceiverId = receiverId,
                Receiver = new User { Id = receiverId },
                IsAnonymous = isAnonymous,
                Description = description,
                Status = KudoStatus.Accepted
            };
        }
    }
}
