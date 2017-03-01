using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class BoardControllerDeleteTests :BoardControllerTestBase
    {
        private Mock<IBoardManager> boardManagerMock = new Mock<IBoardManager>();


        [Fact]
        public void Board_Delete_Returns_Ok()
        {
            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext();
            StatusCodeResult response = controller.Delete(1) as StatusCodeResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Board_Delete_Calls_ManagerDelete_Once_With_Proper_Arguments()
        {
            BoardController controller = new BoardController(boardManagerMock.Object);
            string userId = "adadqwee123";
            controller.ControllerContext = GetControllerContext(userId);

            int idToDelete = 1;

            controller.Delete(idToDelete);

            boardManagerMock.Verify(x => x.Delete(It.Is<string>(i => i == userId), It.Is<int>(i => i == idToDelete)), Times.Once);
        }

        [Fact]
        public void Board_Delete_Returns_NotFound_If_Board_Not_Exist()
        {
            string exceptionMessage = "exception message";
            boardManagerMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<int>())).Throws(new NotFoundException(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext();

            ObjectResult response = controller.Delete(12) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void Board_Delete_Returns_ServerError_When_Exception()
        {
            string exceptionMessage = "exception msg";
            boardManagerMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<int>())).Throws(new Exception(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext();

            ObjectResult response = controller.Delete(1) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void Board_Delete_Returns_Forbiden_If_Try_Delete_Foreign_Board()
        {
            string exceptionMessage = "exception message";
            boardManagerMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<int>())).Throws(new UnauthorizedAccessException(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext();

            ObjectResult response = controller.Delete(1) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Forbidden, (HttpStatusCode)response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }       
    }
}
