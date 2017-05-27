using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
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
    public class JoinRequestControllerAcceptRejectTests : BoardControllerTestBase
    {
        [Fact]
        public void JoinDecision_ValidRequest_ReturnsOkResult()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object);
            controller.WithCurrentUser();

            JoinDecision joinDecision = new JoinDecision(2, true);
            OkResult response = controller.JoinDecision(joinDecision) as OkResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void JoinRecision_Acceptation_CallsAcceptJoin()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object);
            controller.WithCurrentUser("currentUser");

            JoinDecision joinDecision = new JoinDecision(2, true);
            controller.JoinDecision(joinDecision);

            joinManagerMock.Verify(x => x.AcceptJoin(It.Is<int>(i => i == 2), It.Is<string>(i => i == "currentUser")), Times.Once);
        }

        [Fact]
        public void JoinRecision_Rejection_CallsRejectJoin()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object);
            controller.WithCurrentUser("currentUser");

            JoinDecision joinDecision = new JoinDecision(2, false);
            controller.JoinDecision(joinDecision);

            joinManagerMock.Verify(x => x.RejectJoin(It.Is<int>(i => i == 2), It.Is<string>(i => i == "currentUser")), Times.Once);
        }

        [Fact]
        public void JoinDecision_JoinRequestNotExist_ReturnsNotFound()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            joinManagerMock.Setup(x => x.AcceptJoin(It.Is<int>(i => i == 2), It.Is<string>(i => i == "currentUser")))
                .Throws<NotFoundException>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object);
            controller.WithCurrentUser("currentUser");

            JoinDecision joinDecision = new JoinDecision(2, true);
            NotFoundResult response = controller.JoinDecision(joinDecision) as NotFoundResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public void JoinDecision_JoinRequestAlreadyAccepted_ReturnsInternalServerError()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            joinManagerMock.Setup(x => x.AcceptJoin(It.IsAny<int>(), It.IsAny<string>()))
                .Throws<InvalidOperationException>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object);
            controller.WithCurrentUser();

            JoinDecision joinDecision = new JoinDecision(2, true);
            ObjectResult response = controller.JoinDecision(joinDecision) as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void JoinDecision_UserAcceptingForeignRequest_ReturnsUnauthorized()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            joinManagerMock.Setup(x => x.AcceptJoin(It.IsAny<int>(), It.IsAny<string>()))
                .Throws<UnauthorizedAccessException>();
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object);
            controller.WithCurrentUser();

            JoinDecision joinDecision = new JoinDecision(2, true);
            UnauthorizedResult response = controller.JoinDecision(joinDecision) as UnauthorizedResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }

        //[Fact]
        //public void JoinRejection_ValidRequest_ReturnsOkResult()
        //{
        //    Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
        //    JoinRequestController controller = new JoinRequestController(joinManagerMock.Object);
        //    controller.WithCurrentUser();

        //    JoinDecision joinRejection = new JoinDecision("joinId", false);
        //    OkResult response = controller.JoinDecision(JoinDecision) as OkResult;

        //    response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        //}
    }
}
