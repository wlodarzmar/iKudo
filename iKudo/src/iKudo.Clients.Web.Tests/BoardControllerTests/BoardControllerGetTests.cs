using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class BoardControllerGetTests
    {
        private Mock<IManageBoards> boardManagerMock;
        private Mock<IDtoFactory> dtoFactoryMock;

        public BoardControllerGetTests()
        {
            dtoFactoryMock = new Mock<IDtoFactory>();
            boardManagerMock = new Mock<IManageBoards>();
        }

        [Fact]
        public void Get_ReturnsOkWithBoard()
        {
            int boardId = 33;
            Board board = new Board { Name = "name", Description = "desc", CreatorId = "DE%$EDS" };
            boardManagerMock.Setup(x => x.Get(It.Is<int>(c => c == boardId))).Returns(board);
            dtoFactoryMock.Setup(x => x.Create<BoardDTO, Board>(It.IsAny<Board>())).Returns(new BoardDTO());

            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);

            OkObjectResult response = controller.Get(boardId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
            Assert.True(response.Value is BoardDTO);
        }

        [Fact]
        public void Get_CallsGetBoardOnce()
        {
            int boardId = 33;
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);

            controller.Get(boardId);

            boardManagerMock.Verify(x => x.Get(boardId), Times.Once);
        }

        [Fact]
        public void Get_BoardDoesntExist_ReturnsNotFound()
        {
            int boardId = 33;
            boardManagerMock.Setup(x => x.Get(It.Is<int>(c => c == boardId))).Returns((Board)null);
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);

            NotFoundResult response = controller.Get(boardId) as NotFoundResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Get_UnknownExceptionThrown_ReturnsInternalServerError()
        {
            string exceptionMessage = "Nieoczekiwany błąd";
            boardManagerMock.Setup(x => x.Get(It.IsAny<int>()))
                              .Throws(new Exception(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);

            int boardId = 45;
            ObjectResult response = controller.Get(boardId) as ObjectResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }

        [Fact]
        public void GetAll_ReturnsAllBoards()
        {
            ICollection<Board> data = new List<Board> {
                new Board { Id = 1, Name = "board name" },
                new Board { Id = 2, Name = "board name 2" },
                new Board { Id = 3, Name = "board name 3" }
            };
            boardManagerMock.Setup(x => x.GetAll()).Returns(data);
            dtoFactoryMock.Setup(x => x.Create<BoardDTO, Board>(It.IsAny<IEnumerable<Board>>()))
                          .Returns(data.Select(b => new BoardDTO { Id = b.Id, Name = b.Name }).AsEnumerable());

            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);

            OkObjectResult response = controller.GetAll() as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            IEnumerable<BoardDTO> boards = response.Value as IEnumerable<BoardDTO>;
            Assert.Equal(data.Count, boards.Count());
        }

        [Fact]
        public void GetAll_UnknownExceptionThrown_ReturnsInternalServerError()
        {
            string exceptionMessage = "Wystąpił błąd";
            boardManagerMock.Setup(x => x.GetAll()).Throws(new Exception(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);

            ObjectResult response = controller.GetAll() as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }

        [Fact]
        public void GetAll_CallsBoardManagerGetAllOnce()
        {
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);

            controller.GetAll();

            boardManagerMock.Verify(x => x.GetAll(), Times.Once);
        }
    }
}
