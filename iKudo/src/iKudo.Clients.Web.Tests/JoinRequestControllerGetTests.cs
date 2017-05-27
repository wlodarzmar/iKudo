using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
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
    }
}
