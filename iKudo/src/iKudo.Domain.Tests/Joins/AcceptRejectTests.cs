using FluentAssertions;
using iKudo.Domain.Enums;
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
    public class AcceptRejectTests : BaseTest
    {
        [Fact]
        public void AcceptJoin_WithJoinRequest_AcceptsJoinAndReturnsAcceptedJoinRequest()
        {
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            List<JoinRequest> joinRequests = new List<JoinRequest> {
                new JoinRequest {Id =2, BoardId = 1, CandidateId = "userId", Board = new Board { CreatorId = "currentUserId" } }
            };
            DbContext.Fill(joinRequests);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            JoinRequest acceptedJoin = manager.AcceptJoin(2, "currentUserId");

            acceptedJoin.Should().NotBeNull();
            acceptedJoin.StateName.Should().Be("Accepted");
            acceptedJoin.State.Status.Should().Be(JoinStatus.Accepted);
            acceptedJoin.DecisionDate.Should().Be(date);
            acceptedJoin.DecisionUserId.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void AcceptJoin_JoinRequestNotExist_ThrowsNotFound()
        {
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.AcceptJoin(1, "currentUserId"))
                .ShouldThrow<NotFoundException>();
        }

        [Fact]
        public void AcceptJoin_JoinRequestAlreadyAccepted_ThrowsInvalidOperationException()
        {
            JoinRequest joinRequest = new JoinRequest { Id = 2, BoardId = 1, CandidateId = "userId", Board = new Board { CreatorId = "currentUserId", Id = 1 } };
            joinRequest.Accept("currentUserId", DateTime.Now);
            DbContext.Fill(new[] { joinRequest });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.AcceptJoin(2, "currentUserId"))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void AcceptJoin_JoinRequestAlreadyRejected_ThrowsInvalidOperationException()
        {
            JoinRequest joinRequest = new JoinRequest { Id = 2, BoardId = 1, CandidateId = "userId", Board = new Board { Id = 1, CreatorId = "currentUserId" } };
            joinRequest.Reject("currentUserId", DateTime.Now);
            DbContext.Fill(new[] { joinRequest });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.AcceptJoin(2, "currentUserId"))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void AcceptJoin_UserAcceptingForeignJoinRequest_ThrowsUnauthorizedAccessException()
        {
            JoinRequest joinRequest = new JoinRequest { Id = 2, BoardId = 1, CandidateId = "userId", Board = new Board { CreatorId = "creatorId" } };
            DbContext.Fill(new[] { joinRequest });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.AcceptJoin(2, "currentUserId"))
                .ShouldThrow<UnauthorizedAccessException>();
        }

        [Fact]
        public void AcceptJoin_UserAcceptsValidJoin_UserAddedToBoard()
        {
            int boardId = 2;
            string candidateId = "candidate";
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);
            DbContext.Fill(
                new List<JoinRequest> { new JoinRequest(boardId, candidateId, DateTime.Now) { Id = 1, Board = new Board { Id = boardId, CreatorId = "currentUser" } } }
                );

            manager.AcceptJoin(1, "currentUser");

            DbContext.UserBoards.Any(x => x.BoardId == boardId && x.UserId == candidateId).Should().BeTrue();
        }

        [Fact]
        public void RejectJoin_WithValidJoinRequest_RejectsAndReturnsJoinRequest()
        {
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            List<JoinRequest> joinRequests = new List<JoinRequest> {
                new JoinRequest {Id =2 , BoardId = 1, CandidateId = "userId", Board = new Board { CreatorId = "currentUserId" } }
            };
            DbContext.Fill(joinRequests);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            JoinRequest acceptedJoin = manager.RejectJoin(2, "currentUserId");

            acceptedJoin.Should().NotBeNull();            
            acceptedJoin.StateName.Should().Be("Rejected");
            acceptedJoin.State.Status.Should().Be(JoinStatus.Rejected);
            acceptedJoin.DecisionDate.Should().Be(date);
            acceptedJoin.DecisionUserId.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void RejectJoin_JoinRequestNotExist_ThrowsNotFound()
        {
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.RejectJoin(1, "currentUserId"))
                .ShouldThrow<NotFoundException>();
        }

        [Fact]
        public void RejectJoin_JoinRequestAlreadyRejected_ThrowsInvalidOperationException()
        {
            JoinRequest joinRequest = new JoinRequest { Id = 2, BoardId = 1, CandidateId = "userId", Board = new Board { Id = 1, CreatorId = "currentUserId" } };
            joinRequest.Reject("currentUserId", DateTime.Now);
            DbContext.Fill(new[] { joinRequest });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.RejectJoin(2, "currentUserId"))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void RejectJoin_JoinRequestAlreadyAccepted_ThrowsInvalidOperationException()
        {
            JoinRequest joinRequest = new JoinRequest { Id = 1, BoardId = 1, CandidateId = "userId", Board = new Board { CreatorId = "currentUserId", Id = 1 } };
            joinRequest.Accept("currentUserId", DateTime.Now);
            DbContext.Fill(new[] { joinRequest });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.RejectJoin(1, "currentUserId"))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void RejectJoin_UserRejectingForeignJoinRequest_ThrowsUnauthorizedAccessException()
        {
            JoinRequest joinRequest = new JoinRequest { Id = 1, BoardId = 1, CandidateId = "userId", Board = new Board { CreatorId = "creatorId" } };
            DbContext.Fill(new[] { joinRequest });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.Invoking(x => x.RejectJoin(1, "currentUserId"))
                .ShouldThrow<UnauthorizedAccessException>();
        }

        [Fact]
        public void AcceptJoin_JoinAcceptation_AddsNotificationAboutAcceptation()
        {
            int boardId = 1;
            int joinId = 2;
            ICollection<JoinRequest> existingJoins = new List<JoinRequest> {
                new JoinRequest(boardId, "candidate", DateTime.Now) { Id = joinId, Board = new Board { Id = boardId, CreatorId = "creator" } } };
            DbContext.Fill(existingJoins);
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.AcceptJoin(joinId, "creator");

            DbContext.Notifications.Any(x => x.BoardId == boardId &&
                                             x.SenderId == "creator" &&
                                             x.ReceiverId == "candidate" &&
                                             x.Type == NotificationTypes.BoardJoinAccepted &&
                                             x.CreationDate == date)
                                   .Should().BeTrue();
        }

        [Fact]
        public void RejectJoin_JoinRejection_AddsNotificationAboutRejection()
        {
            int boardId = 1;
            int joinId = 2;
            ICollection<JoinRequest> existingJoins = new List<JoinRequest> {
                new JoinRequest(boardId, "candidate", DateTime.Now) { Id = joinId, Board = new Board { Id = boardId, CreatorId = "creator" } } };
            DbContext.Fill(existingJoins);
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            manager.RejectJoin(joinId, "creator");

            DbContext.Notifications.Any(x => x.BoardId == boardId &&
                                             x.SenderId == "creator" &&
                                             x.ReceiverId == "candidate" &&
                                             x.Type == NotificationTypes.BoardJoinRejected &&
                                             x.CreationDate == date)
                                   .Should().BeTrue();
        }
    }
}
