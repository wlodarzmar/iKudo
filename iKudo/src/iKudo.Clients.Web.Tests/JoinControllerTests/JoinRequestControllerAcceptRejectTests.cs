using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class JoinRequestControllerAcceptRejectTests : JoinRequestControllerTestsBase
    {
        [Fact]
        public void JoinDecision_ValidRequest_ReturnsOkResult()
        {
            JoinDecision joinDecision = new JoinDecision(2, true);
            OkResult response = Controller.JoinDecision(joinDecision) as OkResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void JoinRecision_Acceptation_CallsAcceptJoin()
        {
            Controller.WithCurrentUser("currentUser");

            JoinDecision joinDecision = new JoinDecision(2, true);
            Controller.JoinDecision(joinDecision);

            JoinManagerMock.Verify(x => x.AcceptJoin(It.Is<int>(i => i == 2), It.Is<string>(i => i == "currentUser")), Times.Once);
        }

        [Fact]
        public void JoinRecision_Rejection_CallsRejectJoin()
        {
            Controller.WithCurrentUser("currentUser");
            JoinDecision joinDecision = new JoinDecision(2, false);

            Controller.JoinDecision(joinDecision);

            JoinManagerMock.Verify(x => x.RejectJoin(It.Is<int>(i => i == 2), It.Is<string>(i => i == "currentUser")), Times.Once);
        }

        [Fact]
        public void JoinDecision_JoinRequestNotExist_ReturnsNotFound()
        {
            JoinManagerMock.Setup(x => x.AcceptJoin(It.Is<int>(i => i == 2), It.Is<string>(i => i == "currentUser")))
                .Throws<NotFoundException>();
            Controller.WithCurrentUser("currentUser");
            JoinDecision joinDecision = new JoinDecision(2, true);

            NotFoundObjectResult response = Controller.JoinDecision(joinDecision) as NotFoundObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public void JoinDecision_JoinRequestAlreadyAccepted_ReturnsInternalServerError()
        {
            JoinManagerMock.Setup(x => x.AcceptJoin(It.IsAny<int>(), It.IsAny<string>()))
                .Throws<InvalidOperationException>();
            JoinDecision joinDecision = new JoinDecision(2, true);

            ObjectResult response = Controller.JoinDecision(joinDecision) as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.Value.As<ErrorResult>().Message.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void JoinDecision_UserAcceptingForeignRequest_ReturnsUnauthorized()
        {
            JoinManagerMock.Setup(x => x.AcceptJoin(It.IsAny<int>(), It.IsAny<string>()))
                .Throws<UnauthorizedAccessException>();
            JoinDecision joinDecision = new JoinDecision(2, true);

            UnauthorizedResult response = Controller.JoinDecision(joinDecision) as UnauthorizedResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }
    }
}
