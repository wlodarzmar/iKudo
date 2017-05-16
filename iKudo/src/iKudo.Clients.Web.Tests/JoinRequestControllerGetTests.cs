using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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

            OkObjectResult response = controller.GetJoinRequests(userId) as OkObjectResult;

            response.Should().NotBeNull();
            List<JoinRequest> result = response.Value as List<JoinRequest>;
            result.Count.Should().Be(joinRequests.Count);
        }
    }
}
