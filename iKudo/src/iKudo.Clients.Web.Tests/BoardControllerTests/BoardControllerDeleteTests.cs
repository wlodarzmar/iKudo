using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class BoardControllerDeleteTests
    {
        private Mock<IManageBoards> boardManagerMock;
        private Mock<IDtoFactory> dtoFactoryMock;
        private Mock<ILogger<BoardController>> loggerMock;

        public BoardControllerDeleteTests()
        {
            boardManagerMock = new Mock<IManageBoards>();
            dtoFactoryMock = new Mock<IDtoFactory>();
            loggerMock = new Mock<ILogger<BoardController>>();
        }

        [Fact]
        public void Delete_ValidRequest_ReturnsNoContent()
        {
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object, loggerMock.Object);
            controller.WithCurrentUser();
            StatusCodeResult response = controller.Delete(1) as StatusCodeResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NoContent, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Delete_ValidRequest_CallsManagerDeleteOnceWithProperArguments()
        {
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object, loggerMock.Object);
            string userId = "adadqwee123";
            controller.WithCurrentUser(userId);

            int idToDelete = 1;

            controller.Delete(idToDelete);

            boardManagerMock.Verify(x => x.Delete(It.Is<string>(i => i == userId), It.Is<int>(i => i == idToDelete)), Times.Once);
        }

        [Fact]
        public void Delete_BoardDoesntExist_ReturnsNotFound()
        {
            string exceptionMessage = "exception message";
            boardManagerMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<int>())).Throws(new NotFoundException(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object, loggerMock.Object);
            controller.WithCurrentUser();

            ObjectResult response = controller.Delete(12) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }
        
        [Fact]
        public void Delete_UserTriesDeleteForeignBoard_ReturnsForbiden()
        {
            string exceptionMessage = "exception message";
            boardManagerMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<int>())).Throws(new UnauthorizedAccessException(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object, loggerMock.Object);
            controller.WithCurrentUser();

            ObjectResult response = controller.Delete(1) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Forbidden, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }
    }
}
