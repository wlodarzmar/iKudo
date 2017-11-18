using iKudo.Controllers.Api;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private Mock<ILogger<BoardController>> loggerMock;

        public BoardControllerGetTests()
        {
            dtoFactoryMock = new Mock<IDtoFactory>();
            boardManagerMock = new Mock<IManageBoards>();
            loggerMock = new Mock<ILogger<BoardController>>();
        }

        [Fact]
        public void Get_ReturnsOkWithBoard()
        {
            int boardId = 33;
            Board board = new Board { Name = "name", Description = "desc", CreatorId = "DE%$EDS" };
            boardManagerMock.Setup(x => x.Get(It.Is<int>(c => c == boardId))).Returns(board);
            dtoFactoryMock.Setup(x => x.Create<BoardDTO, Board>(It.IsAny<Board>(), It.IsAny<string>())).Returns(new BoardDTO());

            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object, loggerMock.Object);

            OkObjectResult response = controller.Get(boardId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
            Assert.True(response.Value is BoardDTO);
        }

        [Fact]
        public void Get_CallsGetBoardOnce()
        {
            int boardId = 33;
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object, loggerMock.Object);

            controller.Get(boardId);

            boardManagerMock.Verify(x => x.Get(boardId), Times.Once);
        }

        [Fact]
        public void Get_BoardDoesntExist_ReturnsNotFound()
        {
            int boardId = 33;
            boardManagerMock.Setup(x => x.Get(It.Is<int>(c => c == boardId))).Returns((Board)null);
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object, loggerMock.Object);

            NotFoundResult response = controller.Get(boardId) as NotFoundResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void GetAll_ReturnsAllBoards()
        {
            ICollection<Board> data = new List<Board> {
                new Board { Id = 1, Name = "board name" },
                new Board { Id = 2, Name = "board name 2" },
                new Board { Id = 3, Name = "board name 3" }
            };
            boardManagerMock.Setup(x => x.GetAll(It.IsAny<BoardSearchCriteria>())).Returns(data);
            dtoFactoryMock.Setup(x => x.Create<BoardDTO, Board>(It.IsAny<IEnumerable<Board>>()))
                          .Returns(data.Select(b => new BoardDTO { Id = b.Id, Name = b.Name }).AsEnumerable());

            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object, loggerMock.Object);

            OkObjectResult response = controller.GetAll(It.IsAny<BoardSearchCriteria>()) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            IEnumerable<BoardDTO> boards = response.Value as IEnumerable<BoardDTO>;
            Assert.Equal(data.Count, boards.Count());
        }

        [Fact]
        public void GetAll_CallsBoardManagerGetAllOnce()
        {
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object, loggerMock.Object);

            controller.GetAll(It.IsAny<BoardSearchCriteria>());

            boardManagerMock.Verify(x => x.GetAll(It.IsAny<BoardSearchCriteria>()), Times.Once);
        }

        [Fact]
        public void GetAll_WithCreator_CallsManagerWithCreator()
        {
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object, loggerMock.Object);
            BoardSearchCriteria criteria = new BoardSearchCriteria { CreatorId = "creator" };

            controller.GetAll(criteria);

            boardManagerMock.Verify(x => x.GetAll(It.Is<BoardSearchCriteria>(c => c.CreatorId == "creator")));
        }

        [Fact]
        public void GetAll_WithMember_CallsManagerWithMember()
        {
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object, loggerMock.Object);
            BoardSearchCriteria criteria = new BoardSearchCriteria { Member = "user" };

            controller.GetAll(criteria);

            boardManagerMock.Verify(x => x.GetAll(It.Is<BoardSearchCriteria>(c => c.Member == "user")));
        }
    }
}
