using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
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
    public class JoinRequestControllerTests
    {
        private string location = "some location";
        private Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();

        public JoinRequestControllerTests()
        {
            urlHelperMock.Setup(x=>x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
        }

        [Fact]
        public void JoinRequest_Post_ReturnsCreatedResult()
        {
            Mock<IBoardManager> boardManagerMock = new Mock<IBoardManager>();
            boardManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Returns(new JoinRequest());
            JoinRequestController controller = new JoinRequestController(boardManagerMock.Object);

            NewJoinRequest joinRequest = new NewJoinRequest(1, "123");
            controller.Url = urlHelperMock.Object;

            CreatedResult result =  controller.Post(joinRequest) as CreatedResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [Fact]
        public void JoinRequest_Post_ReturnsLocation()
        {
            Mock<IBoardManager> boardManagerMock = new Mock<IBoardManager>();
            boardManagerMock.Setup(x=>x.Join(It.IsAny<int>(), It.IsAny<string>())).Returns(new JoinRequest());
            JoinRequestController controller = new JoinRequestController(boardManagerMock.Object);
            controller.Url = urlHelperMock.Object;

            NewJoinRequest joinRequest = new NewJoinRequest(1, "123");
            CreatedResult result = controller.Post(joinRequest) as CreatedResult;

            result.Location.Should().Be(location);
        }

        [Fact]
        public void JoinRequest_Post_Calls_BoardsJoin()
        {
            NewJoinRequest joinRequest = new NewJoinRequest(1, "123");
            Mock<IBoardManager> boardManagerMock = new Mock<IBoardManager>();
            boardManagerMock.Setup(x=>x.Join(It.IsAny<int>(), It.IsAny<string>())).Returns(new JoinRequest());

            JoinRequestController controller = new JoinRequestController(boardManagerMock.Object);
            controller.Url = urlHelperMock.Object;

            controller.Post(joinRequest);

            boardManagerMock.Verify(x => x.Join(It.Is<int>(b=>b == joinRequest.BoardId), It.Is<string>(c=>c == joinRequest.CandidateId)), Times.Once);
        }

        [Fact]
        public void JoinRequest_Post_Returns_NotFound_IfBoardDoesNotExist()
        {
            NewJoinRequest joinRequest = new NewJoinRequest(1, "123");
            Mock<IBoardManager> boardManagerMock = new Mock<IBoardManager>();
            string exceptionMessage = "message";
            boardManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Throws(new NotFoundException(exceptionMessage));

            JoinRequestController controller = new JoinRequestController(boardManagerMock.Object);
            controller.Url = urlHelperMock.Object;

            ObjectResult result = controller.Post(joinRequest) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Value.As<ErrorResult>().Error.Should().Be(exceptionMessage);
        }

        [Fact]
        public void JoinRequest_Post_Returns_BadRequest_IfValidationError()
        {
            Mock<IBoardManager> boardManagerMock = new Mock<IBoardManager>();

            JoinRequestController controller = new JoinRequestController(boardManagerMock.Object);
            controller.ModelState.AddModelError("property", "error");
            controller.Url = urlHelperMock.Object;

            BadRequestObjectResult result = controller.Post(new JoinRequest()) as BadRequestObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public void JoinRequest_Returns_InternalServerError_If_InvalidOperationExceptionThrown()
        {
            NewJoinRequest joinRequest = new NewJoinRequest(1, "123");
            Mock<IBoardManager> boardManagerMock = new Mock<IBoardManager>();
            string exceptionMessage = "exception message";
            boardManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Throws(new InvalidOperationException(exceptionMessage));

            JoinRequestController controller = new JoinRequestController(boardManagerMock.Object);
            controller.Url = urlHelperMock.Object;

            ObjectResult result = controller.Post(joinRequest) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            result.Value.As<ErrorResult>().Error.Should().Be(exceptionMessage);
        }

        [Fact]
        public void JoinRequest_Returns_InternalServerError_If_GeneralExceptionThrown()
        {
            NewJoinRequest joinRequest = new NewJoinRequest(1, "123");
            Mock<IBoardManager> boardManagerMock = new Mock<IBoardManager>();
            boardManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Throws(new Exception("error"));

            JoinRequestController controller = new JoinRequestController(boardManagerMock.Object);
            controller.Url = urlHelperMock.Object;

            ObjectResult result = controller.Post(joinRequest) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            result.Value.As<ErrorResult>().Error.Should().NotBeNullOrWhiteSpace();
        }
    }
}
