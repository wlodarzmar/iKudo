using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using System.Reflection;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class BoardControllerPutTests
    {
        private Mock<IManageBoards> boardManagerMock;
        private Mock<IDtoFactory> dtoFactoryMock;

        public BoardControllerPutTests()
        {
            boardManagerMock = new Mock<IManageBoards>();
            dtoFactoryMock = new Mock<IDtoFactory>();
        }

        [Fact]
        public void Put_ValidRequest_ReturnsOk()
        {
            BoardDTO board = new BoardDTO { Id = 1, Name = "name", CreationDate = DateTime.Now };

            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            OkResult response = controller.Put(board) as OkResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Put_ValidRequest_CallsBoardManagerUpdateOnce()
        {
            BoardDTO boardDto = new BoardDTO { Id = 1, Name = "name", CreationDate = DateTime.Now };
            dtoFactoryMock.Setup(x => x.Create<Board, BoardDTO>(It.IsAny<BoardDTO>()))
                          .Returns(new Board { Id = boardDto.Id, Name = boardDto.Name, CreationDate = boardDto.CreationDate });
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();

            controller.Put(boardDto);

            boardManagerMock.Verify(x => x.Update(It.Is<Board>(g => g.Id == boardDto.Id && g.Name == boardDto.Name && g.CreationDate == boardDto.CreationDate)), Times.Once);
        }

        [Fact]
        public void Put_NameAlreadyExists_ReturnsConflict()
        {
            BoardDTO board = new BoardDTO { Id = 1, Name = "name", CreationDate = DateTime.Now };
            string exceptionMessage = "board already exist";
            boardManagerMock.Setup(x => x.Update(It.IsAny<Board>())).Throws(new AlreadyExistException(exceptionMessage));

            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();

            ObjectResult response = controller.Put(board) as ObjectResult;

            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }

        [Fact]
        public void Put_InvalidModel_ReturnsBadRequest()
        {
            BoardDTO board = new BoardDTO();
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            controller.ModelState.AddModelError("property", "error");

            BadRequestObjectResult response = controller.Put(board) as BadRequestObjectResult;

            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Put_ThrowsUnknownException_ReturnsInternalServerError()
        {
            string exceptionMessage = "board already exist";
            boardManagerMock.Setup(x => x.Update(It.IsAny<Board>())).Throws(new Exception(exceptionMessage));

            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();

            ObjectResult response = controller.Put(new BoardDTO()) as ObjectResult;

            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }

        [Fact]
        public void Put_BoardNotExist_ReturnsNotFound()
        {
            BoardDTO board = new BoardDTO { Id = 45345, Name = "name", CreationDate = DateTime.Now };
            string exceptionMessage = "board does not exist";
            boardManagerMock.Setup(x => x.Update(It.IsAny<Board>())).Throws(new NotFoundException(exceptionMessage));

            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();

            NotFoundResult response = controller.Put(board) as NotFoundResult;

            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Put_UserTriesEditForeignBoard_ReturnsForbidden()
        {
            BoardDTO board = new BoardDTO { CreatorId = "creatorId", Id = 45345, Name = "name", CreationDate = DateTime.Now };
            string exceptionMessage = "unauthorized exception message";
            boardManagerMock.Setup(x => x.Update(It.IsAny<Board>())).Throws(new UnauthorizedAccessException(exceptionMessage));

            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            ObjectResult response = controller.Put(board) as ObjectResult;

            Assert.Equal(HttpStatusCode.Forbidden, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }
    }
}
