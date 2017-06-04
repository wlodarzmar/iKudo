using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Joins
{
    public class GetJoinRequestsTests : BaseTest
    {
        [Fact]
        public void GetJoins_WithGivenCandidateId_ReturnsValidCollection()
        {
            List<JoinRequest> existingJoinRequests = new List<JoinRequest>() {
                new JoinRequest() { CandidateId = "user1" },
                new JoinRequest() { CandidateId = "user2" },
                new JoinRequest() { CandidateId = "user1" }
            };
            DbContext.Fill(existingJoinRequests);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);
            JoinSearchCriteria criteria = new JoinSearchCriteria { CandidateId = "user1" };

            IEnumerable<JoinRequest> result = manager.GetJoins(criteria);

            result.Count().Should().Be(2);
        }

        [Fact]
        public void GetJoins_WithGivenBoardId_ReturnsValidCollection()
        {
            List<JoinRequest> existingJoinRequests = new List<JoinRequest>() {
                new JoinRequest() { BoardId = 1},
                new JoinRequest() { BoardId = 1},
                new JoinRequest() { BoardId = 2}
            };
            DbContext.Fill(existingJoinRequests);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);
            JoinSearchCriteria criteria = new JoinSearchCriteria { BoardId = 2 };

            IEnumerable<JoinRequest> result = manager.GetJoins(criteria);

            result.Count().Should().Be(1);
        }

        [Fact]
        public void GetJoins_WithGivenStatus_ReturnsValidCollection()
        {
            List<JoinRequest> existingJoinRequests = new List<JoinRequest>() {
                new JoinRequest() { BoardId = 1},
                new JoinRequest() { BoardId = 1},
                new JoinRequest() { BoardId = 2}
            };
            existingJoinRequests[0].Accept("user", DateTime.Now);
            existingJoinRequests[1].Accept("user", DateTime.Now);
            DbContext.Fill(existingJoinRequests);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);
            JoinSearchCriteria criteria = new JoinSearchCriteria { StatusText = "waiting" };

            IEnumerable<JoinRequest> result = manager.GetJoins(criteria);

            result.Count().Should().Be(1);
        }
    }
}
