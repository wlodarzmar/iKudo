using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class BoardControllerGetTests
    {
        Mock<IBoardManager> boardManagerMock = new Mock<IBoardManager>();

        [Fact]
        public void BoardGet_Returns_Ok_With_Board_Object()
        {
            int boardId = 33;
            Board board = new Board { Name = "name", Description = "desc", CreatorId = "DE%$EDS" };
            boardManagerMock.Setup(x => x.Get(It.Is<int>(c => c == boardId))).Returns(board);
            BoardController controller = new BoardController(boardManagerMock.Object);

            OkObjectResult response = controller.Get(boardId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
            Assert.True(response.Value is Board);
        }

        [Fact]
        public void BoardGet_Calls_GetBoard_Once()
        {
            int boardId = 33;
            BoardController controller = new BoardController(boardManagerMock.Object);

            controller.Get(boardId);

            boardManagerMock.Verify(x => x.Get(boardId), Times.Once);
        }

        [Fact]
        public void BoardGet_Returns_NotFound_If_BoardId_Not_Exist()
        {
            int boardId = 33;
            boardManagerMock.Setup(x => x.Get(It.Is<int>(c => c == boardId))).Returns((Board)null);
            BoardController controller = new BoardController(boardManagerMock.Object);

            NotFoundResult response = controller.Get(boardId) as NotFoundResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Board_Get_Returns_Error_On_Unknown_Exception()
        {
            string exceptionMessage = "Nieoczekiwany błąd";
            boardManagerMock.Setup(x => x.Get(It.IsAny<int>()))
                              .Throws(new Exception(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object);

            int boardId = 45;
            ObjectResult response = controller.Get(boardId) as ObjectResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(exceptionMessage, response.Value.ToString());
        }

        [Fact]
        public void Board_GetAll_Returns_All_Companies()
        {
            ICollection<Board> data = new List<Board> {
                new Board { Id = 1, Name = "board name" },
                new Board { Id = 2, Name = "board name 2" },
                new Board { Id = 3, Name = "board name 3" }
            };
            boardManagerMock.Setup(x => x.GetAll()).Returns(data);

            BoardController controller = new BoardController(boardManagerMock.Object);

            OkObjectResult response = controller.GetAll() as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            List<Board> companies = response.Value as List<Board>;
            Assert.Equal(data.Count, companies.Count);
        }

        [Fact]
        public void Board_GetAll_Returns_Error_If_Unknown_Exception()
        {
            string exceptionMessage = "Wystąpił błąd";
            boardManagerMock.Setup(x => x.GetAll()).Throws(new Exception(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object);

            ObjectResult response = controller.GetAll() as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void Board_GetAll_Calls_BoardManagerGetAll_Once()
        {
            BoardController controller = new BoardController(boardManagerMock.Object);

            controller.GetAll();

            boardManagerMock.Verify(x => x.GetAll(), Times.Once);
        }
    }
}
