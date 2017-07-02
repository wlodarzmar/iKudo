﻿using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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

            controller.GetJoinRequests(candidateId: userId);

            joinManagerMock.Verify(x => x.GetJoins(It.Is<JoinSearchCriteria>(c => c.CandidateId == userId)));
        }

        [Fact]
        public void JoinRequestController_ReturnsInternalServerErrorIfException()
        {
            string userId = "userId";
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            joinManagerMock.Setup(x => x.GetJoins(It.IsAny<JoinSearchCriteria>())).Throws(new Exception());
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser(userId);

            ObjectResult response = controller.GetJoinRequests() as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GetJoinRequest_ForGivenBoardId_CallsGetJoinsWithBoardId()
        {
            int boardId = 1;
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);

            controller.GetJoinRequests(boardId);

            joinManagerMock.Verify(x => x.GetJoins(It.Is<JoinSearchCriteria>(c => c.BoardId == boardId)));
        }

        [Fact]
        public void GetJoinRequest_InternalException_ReturnsInternalServerError()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            joinManagerMock.Setup(x => x.GetJoins(It.IsAny<JoinSearchCriteria>())).Throws<Exception>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);

            ObjectResult response = controller.GetJoinRequests(1) as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GetJoinRequest_WithGivenStatus_CallsGetJoinsWithValidCriteria()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);

            controller.GetJoinRequests(1, status: "waiting");

            joinManagerMock.Verify(x => x.GetJoins(It.Is<JoinSearchCriteria>(c => c.Status == JoinStatus.Waiting)));
        }
    }
}