using FluentAssertions;
using iKudo.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class JoinRequestTests
    {
        [Fact]
        public void Accept_SetsProperties()
        {
            JoinRequest joinRequest = new JoinRequest { BoardId = 1, CandidateId = "userid" };
            string currentUser = "currentUser";
            DateTime date = DateTime.Now;

            joinRequest.Accept(currentUser, date);

            joinRequest.DecisionDate.Should().Be(date);
            joinRequest.JoinStatusName.Should().Be("Accepted");
            joinRequest.DecisionUserId.Should().Be(currentUser);
        }

        [Fact]
        public void Reject_SetsProperties()
        {
            JoinRequest joinRequest = new JoinRequest { BoardId = 1, CandidateId = "userid" };
            string currentUser = "currentUser";
            DateTime date = DateTime.Now;

            joinRequest.Reject(currentUser, date);

            joinRequest.DecisionDate.Should().Be(date);
            joinRequest.JoinStatusName.Should().Be("Rejected");
            joinRequest.DecisionUserId.Should().Be(currentUser);
        }

        [Fact]
        public void AcceptSth()
        {
            JoinRequest j = new JoinRequest(1, "asds", DateTime.Now);

            j.Accept("qqq", DateTime.Now);

            j.BaseJoinStatus.Should().BeOfType<Accepted>();
        }
    }
}
