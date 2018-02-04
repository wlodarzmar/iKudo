using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class BoardControllerGetTests : BoardControllerTestsBase
    {
        [Fact]
        public void Get_ReturnsOkWithBoard()
        {
            int boardId = 33;
            Board board = new Board { Name = "name", Description = "desc", CreatorId = "DE%$EDS" };
            BoardManagerMock.Setup(x => x.Get(It.Is<int>(c => c == boardId))).Returns(board);
            DtoFactoryMock.Setup(x => x.Create<BoardDTO, Board>(It.IsAny<Board>(), It.IsAny<string>())).Returns(new BoardDTO());

            OkObjectResult response = Controller.Get(boardId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
            Assert.True(response.Value is BoardDTO);
        }

        [Fact]
        public void Get_CallsGetBoardOnce()
        {
            int boardId = 33;

            Controller.Get(boardId);

            BoardManagerMock.Verify(x => x.Get(boardId), Times.Once);
        }

        [Fact]
        public void Get_BoardDoesntExist_ReturnsNotFound()
        {
            int boardId = 33;
            BoardManagerMock.Setup(x => x.Get(It.Is<int>(c => c == boardId))).Returns((Board)null);

            NotFoundResult response = Controller.Get(boardId) as NotFoundResult;

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
            BoardManagerMock.Setup(x => x.GetAll(It.IsAny<string>(), It.IsAny<BoardSearchCriteria>())).Returns(data);
            DtoFactoryMock.Setup(x => x.Create<BoardDTO, Board>(It.IsAny<IEnumerable<Board>>()))
                          .Returns(data.Select(b => new BoardDTO { Id = b.Id, Name = b.Name }).AsEnumerable());

            OkObjectResult response = Controller.GetAll(It.IsAny<BoardSearchCriteria>()) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            IEnumerable<BoardDTO> boards = response.Value as IEnumerable<BoardDTO>;
            Assert.Equal(data.Count, boards.Count());
        }

        [Fact]
        public void GetAll_CallsBoardManagerGetAllOnce()
        {
            Controller.GetAll(It.IsAny<BoardSearchCriteria>());

            BoardManagerMock.Verify(x => x.GetAll(It.IsAny<string>(), It.IsAny<BoardSearchCriteria>()), Times.Once);
        }

        [Fact]
        public void GetAll_WithCreator_CallsManagerWithCreator()
        {
            BoardSearchCriteria criteria = new BoardSearchCriteria { CreatorId = "creator" };

            Controller.GetAll(criteria);

            BoardManagerMock.Verify(x => x.GetAll(It.IsAny<string>(), It.Is<BoardSearchCriteria>(c => c.CreatorId == "creator")));
        }

        [Fact]
        public void GetAll_WithMember_CallsManagerWithMember()
        {
            BoardSearchCriteria criteria = new BoardSearchCriteria { Member = "user" };

            Controller.GetAll(criteria);

            BoardManagerMock.Verify(x => x.GetAll(It.IsAny<string>(), It.Is<BoardSearchCriteria>(c => c.Member == "user")));
        }
    }
}
