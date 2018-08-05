using iKudo.Domain.Model;
using System;

namespace iKudo.Domain.Tests.Helpers
{
    class BoardHelper
    {
        public static Board CreateBoard()
        {
            return new Board
            {
                Id = 1,
                Name = "name",
                CreatorId = "boardCreatorId",
                AcceptanceType = Enums.AcceptanceType.None,
                CreationDate = DateTime.Now,
                Description = "board description",
                IsPrivate = true,
            };
        }

        public static BoardInvitation CreateBoardInvitation(Board board)
        {
            return new BoardInvitation
            {
                Id = 1,
                Board = board,
                BoardId = board.Id,
                Code = Guid.NewGuid(),
                IsActive = true,
                CreationDate = DateTime.Now,
                CreatorId = board.CreatorId,
                Email = "email@email.com"
            };
        }
    }
}
