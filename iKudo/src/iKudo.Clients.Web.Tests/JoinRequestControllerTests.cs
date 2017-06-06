using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
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
    public class JoinRequestControllerTests
    {
        private string location = "some location";
        private Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();
        private Mock<IDtoFactory> dtoFactoryMock = new Mock<IDtoFactory>();

        public JoinRequestControllerTests()
        {
            urlHelperMock.Setup(x=>x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(location);
        }

        [Fact]
        public void JoinRequest_Post_ReturnsCreatedResult()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            joinManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Returns(new JoinRequest());
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            controller.Url = urlHelperMock.Object;

            CreatedResult result =  controller.Post(1) as CreatedResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [Fact]
        public void JoinRequest_Post_ReturnsLocation()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            joinManagerMock.Setup(x=>x.Join(It.IsAny<int>(), It.IsAny<string>())).Returns(new JoinRequest());
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            controller.Url = urlHelperMock.Object;

            CreatedResult result = controller.Post(1) as CreatedResult;

            result.Location.Should().Be(location);
        }

        [Fact]
        public void JoinRequest_Post_Calls_BoardsJoin()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            joinManagerMock.Setup(x=>x.Join(It.IsAny<int>(), It.IsAny<string>())).Returns(new JoinRequest());
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);
            string candidateId = "ASDS@#!";
            controller.WithCurrentUser(candidateId);
            controller.Url = urlHelperMock.Object;

            int boardId = 1;
            controller.Post(boardId);

            joinManagerMock.Verify(x => x.Join(It.Is<int>(b=>b == boardId), It.Is<string>(c=>c == candidateId)), Times.Once);
        }

        [Fact]
        public void JoinRequest_Post_Returns_NotFound_IfBoardDoesNotExist()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            string exceptionMessage = "message";
            joinManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Throws(new NotFoundException(exceptionMessage));
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            controller.Url = urlHelperMock.Object;

            ObjectResult result = controller.Post(1) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Value.As<ErrorResult>().Error.Should().Be(exceptionMessage);
        }

        [Fact]
        public void JoinRequest_Returns_InternalServerError_If_InvalidOperationExceptionThrown()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            string exceptionMessage = "exception message";
            joinManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Throws(new InvalidOperationException(exceptionMessage));
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            controller.Url = urlHelperMock.Object;

            ObjectResult result = controller.Post(1) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            result.Value.As<ErrorResult>().Error.Should().Be(exceptionMessage);
        }

        [Fact]
        public void JoinRequest_Returns_InternalServerError_If_GeneralExceptionThrown()
        {
            Mock<IManageJoins> joinManagerMock = new Mock<IManageJoins>();
            joinManagerMock.Setup(x => x.Join(It.IsAny<int>(), It.IsAny<string>())).Throws(new Exception("error"));
            JoinRequestController controller = new JoinRequestController(joinManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            controller.Url = urlHelperMock.Object;

            ObjectResult result = controller.Post(1) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            result.Value.As<ErrorResult>().Error.Should().NotBeNullOrWhiteSpace();
        }
    }
}
