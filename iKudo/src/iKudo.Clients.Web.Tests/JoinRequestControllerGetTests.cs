using FluentAssertions;
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
    public class JoinRequestControllerGetTests : BoardControllerTestBase
    {
        [Fact]
        public void JoinRequestController_ReturnsJoinRequests()
        {
            string userId = "userId";
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            ICollection<JoinRequest> joinRequests = new List<JoinRequest> {
                new JoinRequest {BoardId = 1, CandidateId = userId }
            };
            joinManagerMock.Setup(x => x.GetJoinRequests(It.IsAny<string>())).Returns(joinRequests);
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object);
            controller.WithCurrentUser(userId);

            OkObjectResult response = controller.GetJoinRequests() as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            List<JoinRequest> result = response.Value as List<JoinRequest>;
            result.Count.Should().Be(joinRequests.Count);
        }

        [Fact]
        public void JoinRequestController_ReturnsInternalServerErrorIfException()
        {
            string userId = "userId";
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();

            joinManagerMock.Setup(x => x.GetJoinRequests(It.IsAny<string>())).Throws(new Exception());
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object);
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
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object);

            controller.GetJoinRequests(boardId);

            joinManagerMock.Verify(x => x.GetJoins(It.Is<JoinSearchCriteria>(c => c.BoardId == boardId)));
        }

        [Fact]
        public void GetJoinRequest_InternalException_ReturnsInternalServerError()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            joinManagerMock.Setup(x => x.GetJoins(It.IsAny<JoinSearchCriteria>())).Throws<Exception>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object);

            ObjectResult response = controller.GetJoinRequests(1) as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GetJoinRequest_WithGivenStatus_ReturnsValidCollection()
        {
            //int boardId = 1;
            //List<JoinRequest> existingJoins = new List<JoinRequest> {
            //    new JoinRequest(boardId, "can", DateTime.Now),
            //    new JoinRequest(boardId, "can", DateTime.Now),
            //};
            //existingJoins[1].Accept("user", DateTime.Now);
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            //joinManagerMock.Setup(x => x.GetJoins(It.Is<JoinSearchCriteria>(c => c.Status == JoinStatus.Waiting)));
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object);

            controller.GetJoinRequests(1, status: "waiting");

            joinManagerMock.Verify(x => x.GetJoins(It.Is<JoinSearchCriteria>(c => c.Status == JoinStatus.Waiting)));
            //response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            //response.Value.As<IEnumerable<JoinDTO>>().Count().Should().Be(1);
        }
    }
}
