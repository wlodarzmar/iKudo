using FluentAssertions;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Joins
{
    public class JoinTests : BaseTest
    {
        [Fact]
        public void JoinManager_Join_ReturnsJoinRequest()
        {
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            Board board = new Board { CreationDate = DateTime.Now, CreatorId = "123", Id = 1, Name = "name" };
            DbContext.Fill(new List<Board> { board });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);
            string candidateId = "asdasd";

            JoinRequest joinRequest = manager.Join(board.Id, candidateId);

            joinRequest.Should().NotBeNull();
            joinRequest.CandidateId.Should().Be(candidateId);
            joinRequest.CreationDate.Should().Be(date);
            joinRequest.IsAccepted.Should().BeNull();
            joinRequest.BoardId.Should().Be(board.Id);
        }

        [Fact]
        public void JoinManager_Join_AddsJoinRequestToBoard()
        {
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            Board board = new Board { CreationDate = DateTime.Now, CreatorId = "123", Id = 1, Name = "name" };
            DbContext.Fill(new List<Board> { board });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);
            string candidateId = "asdasd";

            JoinRequest joinRequest = manager.Join(board.Id, candidateId);

            board.JoinRequests.Count.ShouldBeEquivalentTo(1);
        }

        [Fact]
        public void JoinManager_Join_SavesChanges()
        {
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            Board board = new Board { CreationDate = DateTime.Now, CreatorId = "123", Id = 1, Name = "name" };
            DbContext.Fill(new List<Board> { board });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);
            string candidateId = "asdasd";

            JoinRequest joinRequest = manager.Join(board.Id, candidateId);

            JoinRequest addedRequest = DbContext.JoinRequests.FirstOrDefault(x => x.Id == joinRequest.Id);

            addedRequest.Should().NotBeNull();
        }

        [Fact]
        public void JoinManager_Join_ThrowsBoardNotFoundExceptionIfBoardNotExist()
        {
            TimeProviderMock.Setup(x => x.Now()).Returns(DateTime.Now);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);
            string candidateId = "asdasd";

            manager.Invoking(x => x.Join(123, candidateId))
                   .ShouldThrow<NotFoundException>();
        }

        [Fact]
        public void JoinManager_Join_ThrowsInvalidOperationExceptionIfCreatorAttemptsToJoinBoard()
        {
            TimeProviderMock.Setup(x => x.Now()).Returns(DateTime.Now);
            Board board = new Board { CreationDate = DateTime.Now, CreatorId = "123", Id = 1, Name = "name" };
            DbContext.Fill(new List<Board> { board });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.Join(board.Id, board.CreatorId))
                   .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void JoinManager_Join_ThrowsInvalidOperationExceptionIfAlreadyExistsNotAcceptedJoinRequest()
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
            DbContext.Fill(new List<Board> { board });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.Join(board.Id, candidateId))
                   .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void JoinManager_Join_ThrowsInvalidOperationExceptionIfCandidateAlreadyJoinedBoard()
        {
            TimeProviderMock.Setup(x => x.Now()).Returns(DateTime.Now);
            int boardId = 1;
            string candidateId = "candidateID";

            UserBoard userBoard = new UserBoard(candidateId, boardId);
            Board board = new Board
            {
                CreationDate = DateTime.Now,
                CreatorId = "123",
                Id = boardId,
                Name = "name",
                UserBoards = new List<UserBoard> { userBoard }
            };
            DbContext.Fill(new List<Board> { board });

            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            Action action = () => manager.Join(boardId, candidateId);

            action.ShouldThrow<InvalidOperationException>();
        }
    }
}
