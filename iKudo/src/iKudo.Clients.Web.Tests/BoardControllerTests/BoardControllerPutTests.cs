using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using System.Reflection;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class BoardControllerPutTests : BoardControllerTestBase
    {
        Mock<IManageBoards> boardManagerMock = new Mock<IManageBoards>();

        [Fact]
        public void BoardController_Put_Returns_Ok_After_Succeeded_Modification()
        {
            Board board = new Board { Id = 1, Name = "name", CreationDate = DateTime.Now };

            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext(null);
            OkResult response = controller.Put(board) as OkResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void BoardController_Put_Calls_BoardManager_Update_Once()
        {
            Board board = new Board { Id = 1, Name = "name", CreationDate = DateTime.Now };
            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext(null);

            controller.Put(board);

            boardManagerMock.Verify(x => x.Update(It.Is<Board>(g => g == board)), Times.Once);
        }

        [Fact]
        public void BoardController_Put_Returns_Conflict_If_Name_Exist()
        {
            Board board = new Board { Id = 1, Name = "name", CreationDate = DateTime.Now };
            string exceptionMessage = "board already exist";
            boardManagerMock.Setup(x => x.Update(It.IsAny<Board>())).Throws(new AlreadyExistException(exceptionMessage));

            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext(null);

            ObjectResult response = controller.Put(board) as ObjectResult;

            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }

        [Fact]
        public void BoardController_Put_Returns_BadRequest_If_Model_Is_Invalid()
        {
            Board board = new Board();
            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext(null);            
            controller.ModelState.AddModelError("property", "error");

            BadRequestObjectResult response = controller.Put(board) as BadRequestObjectResult;

            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void BoardController_Returns_ServerError_If_Exception()
        {
            string exceptionMessage = "board already exist";
            boardManagerMock.Setup(x => x.Update(It.IsAny<Board>())).Throws(new Exception(exceptionMessage));

            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext(null);

            ObjectResult response = controller.Put(new Board()) as ObjectResult;

            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }

        [Fact]
        public void BoardController_Put_Returns_NotFound_If_Board_Not_Exist()
        {
            Board board = new Board { Id = 45345, Name = "name", CreationDate = DateTime.Now };
            string exceptionMessage = "board does not exist";
            boardManagerMock.Setup(x => x.Update(It.IsAny<Board>())).Throws(new NotFoundException(exceptionMessage));

            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext(null);

            NotFoundResult response = controller.Put(board) as NotFoundResult;

            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void BoardController_Returns_Forbidden_If_Edit_Foreign_Board()
        {
            Board board = new Board {CreatorId = "creatorId", Id = 45345, Name = "name", CreationDate = DateTime.Now };
            string exceptionMessage = "unauthorized exception message";
            boardManagerMock.Setup(x => x.Update(It.IsAny<Board>())).Throws(new UnauthorizedAccessException(exceptionMessage));

            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.ControllerContext = GetControllerContext();
            ObjectResult response = controller.Put(board) as ObjectResult;

            Assert.Equal(HttpStatusCode.Forbidden, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }
    }
}
