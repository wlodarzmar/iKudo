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
                new JoinRequest(1, "user1", DateTime.Now),
                new JoinRequest(2, "user2", DateTime.Now),
                new JoinRequest(3, "user1", DateTime.Now),
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
                new JoinRequest(1, "user", DateTime.Now),
                new JoinRequest(2, "user", DateTime.Now),
                new JoinRequest(1, "user", DateTime.Now),
            };
            DbContext.Fill(existingJoinRequests);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);
            JoinSearchCriteria criteria = new JoinSearchCriteria { BoardId = 2 };

            IEnumerable<JoinRequest> result = manager.GetJoins(criteria);

            result.Count().Should().Be(1);
        }

        [Fact]
        public void GetJoins_WithGivenBoardId_ReturnsValidCollection2()
        {
            List<JoinRequest> existingJoinRequests = new List<JoinRequest>() {
                new JoinRequest(4, "user", DateTime.Now),
                new JoinRequest(5, "user", DateTime.Now),
                new JoinRequest(16, "user", DateTime.Now),
            };
            DbContext.Fill(existingJoinRequests);
            IManageJoins manager = new JoinManager(DbContext, TimeProviderMock.Object);
            JoinSearchCriteria criteria = new JoinSearchCriteria { BoardId = 16 };

            IEnumerable<JoinRequest> result = manager.GetJoins(criteria);

            result.Count().Should().Be(1);
        }

        [Fact]
        public void GetJoins_WithGivenStatus_ReturnsValidCollection()
        {
            List<JoinRequest> existingJoinRequests = new List<JoinRequest>() {
                new JoinRequest(1, string.Empty, DateTime.Now),
                new JoinRequest(1, string.Empty, DateTime.Now),
                new JoinRequest(2, string.Empty, DateTime.Now)
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
