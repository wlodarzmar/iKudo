using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Joins
{
    public class GetJoinRequestsTests : JoinRequestTestsBase
    {
        [Fact]
        public void GetJoins_WithGivenCandidateId_ReturnsValidCollection()
        {
            List<JoinRequest> existingJoinRequests = new List<JoinRequest>() {
                CreateJoinRequest(1, "user1"),
                CreateJoinRequest(2, "user2"),
                CreateJoinRequest(3, "user1"),
            };
            DbContext.Fill(existingJoinRequests);

            JoinSearchCriteria criteria = new JoinSearchCriteria { CandidateId = "user1" };

            IEnumerable<JoinRequest> result = Manager.GetJoins(criteria);

            result.Count().Should().Be(2);
        }

        [Fact]
        public void GetJoins_WithGivenBoardId_ReturnsValidCollection()
        {
            List<JoinRequest> existingJoinRequests = new List<JoinRequest>() {
                CreateJoinRequest(1,"user"),
                CreateJoinRequest(2,"user"),
                CreateJoinRequest(1,"user"),
            };
            DbContext.Fill(existingJoinRequests);
            JoinSearchCriteria criteria = new JoinSearchCriteria { BoardId = 2 };

            IEnumerable<JoinRequest> result = Manager.GetJoins(criteria);

            result.Count().Should().Be(1);
        }

        [Fact]
        public void GetJoins_WithGivenBoardId_ReturnsValidCollection2()
        {
            List<JoinRequest> existingJoinRequests = new List<JoinRequest>() {
                CreateJoinRequest(4, "user"),
                CreateJoinRequest(5, "user"),
                CreateJoinRequest(16, "user"),
            };
            DbContext.Fill(existingJoinRequests);
            JoinSearchCriteria criteria = new JoinSearchCriteria { BoardId = 16 };

            IEnumerable<JoinRequest> result = Manager.GetJoins(criteria);

            result.Count().Should().Be(1);
        }

        [Fact]
        public void GetJoins_WithGivenStatus_ReturnsValidCollection()
        {
            List<JoinRequest> existingJoinRequests = new List<JoinRequest>() {
                CreateJoinRequest(1, "user"),
                CreateJoinRequest(1, "user"),
                CreateJoinRequest(2, "user"),
            };
            existingJoinRequests[0].Accept("user", DateTime.Now);
            existingJoinRequests[1].Accept("user", DateTime.Now);
            DbContext.Fill(existingJoinRequests);
            JoinSearchCriteria criteria = new JoinSearchCriteria { StatusText = "waiting" };

            IEnumerable<JoinRequest> result = Manager.GetJoins(criteria);

            result.Count().Should().Be(1);
        }
    }
}
