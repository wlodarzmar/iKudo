﻿using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class JoinRequestControllerGetTests
    {
        private Mock<IDtoFactory> dtoFactoryMock;

        public JoinRequestControllerGetTests()
        {
            dtoFactoryMock = new Mock<IDtoFactory>();
        }

        [Fact]
        public void GetJoinRequests_WithGivenCandidateId_CallsGetJoinsWithCandidateId()
        {
            string userId = "userId";
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);

            controller.GetJoinRequests(new JoinSearchCriteria { CandidateId = userId });

            joinManagerMock.Verify(x => x.GetJoins(It.Is<JoinSearchCriteria>(c => c.CandidateId == userId)));
        }
        
        [Fact]
        public void GetJoinRequest_ForGivenBoardId_CallsGetJoinsWithBoardId()
        {
            int boardId = 1;
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);

            controller.GetJoinRequests(new JoinSearchCriteria { BoardId = boardId });

            joinManagerMock.Verify(x => x.GetJoins(It.Is<JoinSearchCriteria>(c => c.BoardId == boardId)));
        }

        [Fact]
        public void GetJoinRequest_WithGivenStatus_CallsGetJoinsWithValidCriteria()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);

            controller.GetJoinRequests(new JoinSearchCriteria { BoardId = 1, StatusText = "waiting" });

            joinManagerMock.Verify(x => x.GetJoins(It.Is<JoinSearchCriteria>(c => c.Status == JoinStatus.Waiting)));
        }
    }
}
