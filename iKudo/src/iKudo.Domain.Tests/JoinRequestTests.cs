using FluentAssertions;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class JoinRequestTests : BoardTestsBase
    {
        [Fact]
        public void BoardManager_Join_ReturnsJoinRequest()
        {
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            Board board = new Board { CreationDate = DateTime.Now, CreatorId = "123", Id = 1, Name = "name" };
            FillContext(new List<Board> { board });
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);
            string candidateId = "asdasd";

            JoinRequest joinRequest = manager.Join(board.Id, candidateId);

            joinRequest.Should().NotBeNull();
            joinRequest.CandidateId.Should().Be(candidateId);
            joinRequest.CreationDate.Should().Be(date);
            joinRequest.IsAccepted.Should().Be(false);
            joinRequest.BoardId.Should().Be(board.Id);
        }

        [Fact]
        public void BoardManager_Join_AddsJoinRequestToBoard()
        {
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            Board board = new Board { CreationDate = DateTime.Now, CreatorId = "123", Id = 1, Name = "name" };
            FillContext(new List<Board> { board });
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);
            string candidateId = "asdasd";

            JoinRequest joinRequest = manager.Join(board.Id, candidateId);

            board.JoinRequests.Count.ShouldBeEquivalentTo(1);
        }

        [Fact]
        public void BoardManager_Join_SavesChanges()
        {
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            Board board = new Board { CreationDate = DateTime.Now, CreatorId = "123", Id = 1, Name = "name" };
            FillContext(new List<Board> { board });
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);
            string candidateId = "asdasd";

            JoinRequest joinRequest = manager.Join(board.Id, candidateId);

            JoinRequest addedRequest = DbContext.JoinRequests.FirstOrDefault(x => x.Id == joinRequest.Id);

            addedRequest.Should().NotBeNull();
        }

        [Fact]
        public void BoardManager_Join_ThrowsBoardNotFoundExceptionIfBoardNotExist()
        {
            TimeProviderMock.Setup(x => x.Now()).Returns(DateTime.Now);
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);
            string candidateId = "asdasd";

            manager.Invoking(x => x.Join(123, candidateId))
                   .ShouldThrow<NotFoundException>();
        }

        [Fact]
        public void BoardManager_Join_ThrowsInvalidOperationExceptionIfCreatorAttemptsToJoinBoard()
        {
            TimeProviderMock.Setup(x => x.Now()).Returns(DateTime.Now);
            Board board = new Board { CreationDate = DateTime.Now, CreatorId = "123", Id = 1, Name = "name" };
            FillContext(new List<Board> { board });
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.Join(board.Id, board.CreatorId))
                   .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void BoardManager_Join_ThrowsInvalidOperationExceptionIfAlreadyExistsNotAcceptedJoinRequest()
        {
            TimeProviderMock.Setup(x => x.Now()).Returns(DateTime.Now);
            int boardId = 1;
            string candidateId = "candidateID";
            JoinRequest existingJoinRequest = new JoinRequest { BoardId = boardId, CandidateId = candidateId, CreationDate = DateTime.Now };
            Board board = new Board
            {
                CreationDate = DateTime.Now,
                CreatorId = "123",
                Id = boardId,
                Name = "name",
                JoinRequests = new List<JoinRequest> { existingJoinRequest }
            };
            FillContext(new List<Board> { board });
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.Join(board.Id, candidateId))
                   .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void BoardManager_Join_ThrowsInvalidOperationExceptionIfCandidateAlreadyJoinedBoard()
        {
            TimeProviderMock.Setup(x => x.Now()).Returns(DateTime.Now);
            int boardId = 1;
            string candidateId = "candidateID";

            UserBoard userBoard = new UserBoard { UserId = candidateId, BoardId = boardId };
            Board board = new Board
            {
                CreationDate = DateTime.Now,
                CreatorId = "123",
                Id = boardId,
                Name = "name",
                UserBoards = new List<UserBoard> { userBoard }
            };
            FillContext(new List<Board> { board });

            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            Action action = () => manager.Join(boardId, candidateId);

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void BoardManager_GetJoinRequests_ReturnsValidRequests()
        {
            string userId = "user1";
            ICollection<JoinRequest> existingJoinRequests = new List<JoinRequest> {
                new JoinRequest { BoardId = 1, CandidateId = userId },
                new JoinRequest { BoardId = 2, CandidateId = userId },
                new JoinRequest { BoardId = 2, CandidateId = "user2" },
            };
            FillContext(existingJoinRequests);
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            ICollection<JoinRequest> result = manager.GetJoinRequests(userId);

            result.Count.Should().Be(2);
        }

        [Fact]
        public void BoardManager_GetJoinRequests_ReturnsEmptyListIfNoValidRequests()
        {
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            ICollection<JoinRequest> result = manager.GetJoinRequests("user");

            result.Count.Should().Be(0);
        }
    }
}
