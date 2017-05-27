using FluentAssertions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Joins
{
    public class GetJoinRequestsTests : BaseTest
    {
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

            IEnumerable<JoinRequest> result = manager.GetJoinRequests(userId);

            result.Count().Should().Be(2);
        }

        [Fact]
        public void JoinManager_GetJoinRequests_ReturnsEmptyListIfNoValidRequests()
        {
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            IEnumerable<JoinRequest> result = manager.GetJoinRequests("user");

            result.Count().Should().Be(0);
        }

        [Fact]
        public void JoinManager_GetJoinRequests_ReturnsJoinsForGivenBoardId()
        {
            var board = new Board { Id = 1, Name = "name1" };
            board.JoinRequests.Add(new JoinRequest(1, "candidate1"));
            board.JoinRequests.Add(new JoinRequest(1, "candidate2"));

            var board2 = new Board { Id = 2, Name = "name2" };
            board2.JoinRequests.Add(new JoinRequest(2, "candidate3"));
            board2.JoinRequests.Add(new JoinRequest(2, "candidate4"));

            DbContext.Fill(new List<Board> { board, board2 });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            IEnumerable<JoinRequest> joinRequests = manager.GetJoinRequests(1);

            joinRequests.Count().Should().Be(2);
        }

        [Fact]
        public void JoinManager_GetJoinRequests_ReturnsEmptyCollection()
        {
            var board = new Board { Id = 1, Name = "name1" };
            board.JoinRequests.Add(new JoinRequest(1, "candidate1"));
            board.JoinRequests.Add(new JoinRequest(1, "candidate2"));
            DbContext.Fill(new List<Board> { board });
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);

            IEnumerable<JoinRequest> joinRequests = manager.GetJoinRequests(2);

            joinRequests.Count().Should().Be(0);
        }

    }
}
