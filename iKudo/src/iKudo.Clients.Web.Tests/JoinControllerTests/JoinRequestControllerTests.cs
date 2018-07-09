using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class JoinRequestControllerTests : JoinRequestControllerTestsBase
    {
        private readonly string location = "some location";
        private readonly Mock<IUrlHelper> urlHelperMock;

        public JoinRequestControllerTests()
        {
            urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
        }

        [Fact]
        public void JoinRequest_Post_ReturnsCreatedResult()
        {
            JoinManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Returns(new JoinRequest());
            Controller.Url = urlHelperMock.Object;

            CreatedResult result = Controller.Post(1) as CreatedResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [Fact]
        public void JoinRequest_Post_ReturnsLocation()
        {
            JoinManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Returns(new JoinRequest());
            Controller.Url = urlHelperMock.Object;

            CreatedResult result = Controller.Post(1) as CreatedResult;

            result.Location.Should().Be(location);
        }

        [Fact]
        public void JoinRequest_Post_Calls_BoardsJoin()
        {
            JoinManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Returns(new JoinRequest());
            string candidateId = "ASDS@#!";
            Controller.WithCurrentUser(candidateId);
            Controller.Url = urlHelperMock.Object;

            int boardId = 1;
            Controller.Post(boardId);

            JoinManagerMock.Verify(x => x.Join(It.Is<int>(b => b == boardId), It.Is<string>(c => c == candidateId)), Times.Once);
        }

        [Fact]
        public void JoinRequest_Post_Returns_NotFound_IfBoardDoesNotExist()
        {
            string exceptionMessage = "message";
            JoinManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Throws(new NotFoundException(exceptionMessage));
            Controller.Url = urlHelperMock.Object;

            ObjectResult result = Controller.Post(1) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Value.As<ErrorResult>().Message.Should().Be(exceptionMessage);
        }

        [Fact]
        public void JoinRequest_Returns_InternalServerError_If_InvalidOperationExceptionThrown()
        {
            string exceptionMessage = "exception message";
            JoinManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Throws(new InvalidOperationException(exceptionMessage));
            Controller.Url = urlHelperMock.Object;

            ObjectResult result = Controller.Post(1) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            result.Value.As<ErrorResult>().Message.Should().Be(exceptionMessage);
        }
    }
}
