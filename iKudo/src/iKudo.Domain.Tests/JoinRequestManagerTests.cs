using FluentAssertions;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class JoinRequestManagerTests : BoardTestsBase
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

            UserBoard userBoard = new UserBoard { UserId = candidateId, BoardId = boardId };
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

        [Fact]
        public void JoinManager_GetJoinRequests_ReturnsValidRequests()
        {
            string userId = "user1";
            ICollection<JoinRequest> existingJoinRequests = new List<JoinRequest> {
                new JoinRequest { BoardId = 1, CandidateId = userId },
                new JoinRequest { BoardId = 2, CandidateId = userId },
                new JoinRequest { BoardId = 2, CandidateId = "user2" },
            };
            DbContext.Fill(existingJoinRequests);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            ICollection<JoinRequest> result = manager.GetJoinRequests(userId);

            result.Count.Should().Be(2);
        }

        [Fact]
        public void JoinManager_GetJoinRequests_ReturnsEmptyListIfNoValidRequests()
        {
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            ICollection<JoinRequest> result = manager.GetJoinRequests("user");

            result.Count.Should().Be(0);
        }

        [Fact]
        public void AcceptJoin_WithJoinRequest_AcceptsJoinAndReturnsAcceptedJoinRequest()
        {
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            List<JoinRequest> joinRequests = new List<JoinRequest> {
                new JoinRequest {Id ="joinId", BoardId = 1, CandidateId = "userId", Board = new Board { CreatorId = "currentUserId" } }
            };
            DbContext.Fill(joinRequests);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            JoinRequest acceptedJoin = manager.AcceptJoin("joinId", "currentUserId");

            acceptedJoin.Should().NotBeNull();
            acceptedJoin.IsAccepted.Should().BeTrue();
            acceptedJoin.DecisionDate.Should().Be(date);
            acceptedJoin.DecisionUserId.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void AcceptJoin_JoinRequestNotExist_ThrowsNotFound()
        {
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.AcceptJoin("notExistingJoinId", "currentUserId"))
                .ShouldThrow<NotFoundException>();
        }

        [Fact]
        public void AcceptJoin_JoinRequestAlreadyAccepted_ThrowsInvalidOperationException()
        {
            JoinRequest joinRequest = new JoinRequest { Id = "joinId", BoardId = 1, CandidateId = "userId" };
            joinRequest.Accept("currentUserId", DateTime.Now);
            DbContext.Fill(new[] { joinRequest });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.AcceptJoin("joinId", "currentUserId"))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void AcceptJoin_JoinRequestAlreadyRejected_ThrowsInvalidOperationException()
        {
            JoinRequest joinRequest = new JoinRequest { Id = "joinId", BoardId = 1, CandidateId = "userId" };
            joinRequest.Reject("currentUserId", DateTime.Now);
            DbContext.Fill(new[] { joinRequest });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.AcceptJoin("joinId", "currentUserId"))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void AcceptJoin_UserAcceptingForeignJoinRequest_ThrowsUnauthorizedAccessException()
        {
            JoinRequest joinRequest = new JoinRequest { Id = "joinId", BoardId = 1, CandidateId = "userId", Board = new Board { CreatorId = "creatorId" } };
            DbContext.Fill(new[] { joinRequest });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.AcceptJoin("joinId", "currentUserId"))
                .ShouldThrow<UnauthorizedAccessException>();
        }

        [Fact]
        public void RejectJoin_WithValidJoinRequest_RejectsAndReturnsJoinRequest()
        {
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            List<JoinRequest> joinRequests = new List<JoinRequest> {
                new JoinRequest {Id ="joinId", BoardId = 1, CandidateId = "userId", Board = new Board { CreatorId = "currentUserId" } }
            };
            DbContext.Fill(joinRequests);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            JoinRequest acceptedJoin = manager.RejectJoin("joinId", "currentUserId");

            acceptedJoin.Should().NotBeNull();
            acceptedJoin.IsAccepted.Should().BeFalse();
            acceptedJoin.DecisionDate.Should().Be(date);
            acceptedJoin.DecisionUserId.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void RejectJoin_JoinRequestNotExist_ThrowsNotFound()
        {
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.RejectJoin("notExistingJoinId", "currentUserId"))
                .ShouldThrow<NotFoundException>();
        }

        [Fact]
        public void RejectJoin_JoinRequestAlreadyRejected_ThrowsInvalidOperationException()
        {
            JoinRequest joinRequest = new JoinRequest { Id = "joinId", BoardId = 1, CandidateId = "userId" };
            joinRequest.Reject("currentUserId", DateTime.Now);
            DbContext.Fill(new[] { joinRequest });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.RejectJoin("joinId", "currentUserId"))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void RejectJoin_JoinRequestAlreadyAccepted_ThrowsInvalidOperationException()
        {
            JoinRequest joinRequest = new JoinRequest { Id = "joinId", BoardId = 1, CandidateId = "userId" };
            joinRequest.Accept("currentUserId", DateTime.Now);
            DbContext.Fill(new[] { joinRequest });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.RejectJoin("joinId", "currentUserId"))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void RejectJoin_UserRejectingForeignJoinRequest_ThrowsUnauthorizedAccessException()
        {
            JoinRequest joinRequest = new JoinRequest { Id = "joinId", BoardId = 1, CandidateId = "userId", Board = new Board { CreatorId = "creatorId" } };
            DbContext.Fill(new[] { joinRequest });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.RejectJoin("joinId", "currentUserId"))
                .ShouldThrow<UnauthorizedAccessException>();
        }
    }
}
