using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class JoinRequestControllerGetTests : BoardControllerTestBase
    {
        [Fact]
        public void JoinRequestController_ReturnsJoinRequests()
        {
            string userId = "userId";
            Mock<IBoardManager> boardManagerMock = new Mock<IBoardManager>();
            ICollection<JoinRequest> joinRequests = new List<JoinRequest> {
                new JoinRequest {BoardId = 1, CandidateId = userId }
            };
            boardManagerMock.Setup(x => x.GetJoinRequests(It.IsAny<string>())).Returns(joinRequests);
            JoinRequestController controller = new JoinRequestController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext(userId);

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
            Mock<IBoardManager> boardManagerMock = new Mock<IBoardManager>();

            boardManagerMock.Setup(x => x.GetJoinRequests(It.IsAny<string>())).Throws(new Exception());
            JoinRequestController controller = new JoinRequestController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext(userId);

            ObjectResult response = controller.GetJoinRequests() as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrWhiteSpace();
        }
    }
}
