using FluentAssertions;
using iKudo.Common;
using iKudo.Domain.Enums;
using iKudo.Domain.Logic;
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
            joinRequest.StateName.Should().Be("Accepted");
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
            joinRequest.StateName.Should().Be("Rejected");
            joinRequest.DecisionUserId.Should().Be(currentUser);
        }

        [Fact]
        public void JoinRequest_Accept_ShouldBeInAcceptedState()
        {
            JoinRequest joinRequest = new JoinRequest(1, "asds", DateTime.Now);

            joinRequest.Accept("qqq", DateTime.Now);

            joinRequest.State.Should().BeOfType<Accepted>();
        }

        [Fact]
        public void JoinRequest_Accept_ShouldSetUserDecision()
        {
            JoinRequest joinRequest = new JoinRequest(1, "asds", DateTime.Now);

            joinRequest.Accept("qqq", DateTime.Now);

            joinRequest.DecisionUserId.Should().Be("qqq");
        }

        [Fact]
        public void JoinRequest_Accept_ShouldSetDecisionDate()
        {
            JoinRequest joinRequest = new JoinRequest(1, "asds", DateTime.Now);
            DateTime date = DateTime.Now;
            joinRequest.Accept("qqq", date);

            joinRequest.DecisionDate.Should().Be(date);
        }

        [Fact]
        public void JoinRequest_Reject_ShouldBeInRejectedState()
        {
            JoinRequest joinRequest = new JoinRequest(1, "asds", DateTime.Now);

            joinRequest.Reject("qqq", DateTime.Now);

            joinRequest.State.Should().BeOfType<Rejected>();
        }

        [Fact]
        public void JoinRequest_Reject_ShouldSetUserDecision()
        {
            JoinRequest joinRequest = new JoinRequest(1, "asds", DateTime.Now);

            joinRequest.Reject("qqq", DateTime.Now);

            joinRequest.DecisionUserId.Should().Be("qqq");
        }

        [Fact]
        public void JoinRequest_Reject_ShouldSetDecisionDate()
        {
            JoinRequest joinRequest = new JoinRequest(1, "asds", DateTime.Now);
            DateTime date = DateTime.Now;
            joinRequest.Reject("qqq", date);

            joinRequest.DecisionDate.Should().Be(date);
        }

        [Fact]
        public void JoinRequest_DerivedClass_ProperlySetsStateByName()
        {
            JoinRequestChild join = new JoinRequestChild(JoinStatus.Accepted.GetDisplayName());
            join.State.Should().BeOfType<Accepted>();
            join.State.Name.Should().Be(JoinStatus.Accepted.GetDisplayName());
        }

        class JoinRequestChild : JoinRequest
        {
            public JoinRequestChild(string stateName)
            {
                StateName = stateName;
            }
        }
    }
}
