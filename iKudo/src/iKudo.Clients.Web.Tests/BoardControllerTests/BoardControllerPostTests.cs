using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class BoardControllerPostTests : BoardControllerTestsBase
    {
        private string locationUrl = "http://location/";
        private Mock<IUrlHelper> urlHelperMock;

        public BoardControllerPostTests()
        {
            urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(locationUrl);
        }

        [Fact]
        public void Post_ValidRequest_ReturnsCreatedStatus()
        {
            Controller.Url = urlHelperMock.Object;
            BoardDTO board = new BoardDTO();

            CreatedResult response = Controller.Post(board) as CreatedResult;

            Assert.True(response.StatusCode == (int)HttpStatusCode.Created);
        }

        [Fact]
        public void Post_ValidRequest_ReturnsLocation()
        {
            Controller.Url = urlHelperMock.Object;
            BoardDTO board = new BoardDTO();

            CreatedResult response = Controller.Post(board) as CreatedResult;

            Assert.Equal(locationUrl, response.Location);
        }

        [Fact]
        public void Post_ValidRequest_CallsAddOnce()
        {
            Controller.Url = urlHelperMock.Object;

            BoardDTO board = new BoardDTO();

            Controller.Post(board);

            BoardManagerMock.Verify(x => x.Add(It.IsAny<Board>()), Times.Once);
        }

        [Fact]
        public void Post_NameAlreadyExists_ReturnsConflict()
        {
            string exceptionMessage = "Obiekt już istnieje";
            BoardManagerMock.Setup(x => x.Add(It.IsAny<Board>()))
                              .Throws(new AlreadyExistException(exceptionMessage));
            BoardDTO board = new BoardDTO() { Name = "existing name" };

            ObjectResult response = Controller.Post(board) as ObjectResult;

            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }

        [Fact]
        public void Post_CallsAddWithCurrentUserIdAsCreator()
        {
            string userId = "userId";
            BoardDTO board = new BoardDTO { Name = "name" };
            DtoFactoryMock.Setup(x => x.Create<Board, BoardDTO>(It.IsAny<BoardDTO>())).Returns(new Board { Name = board.Name, CreatorId = userId });
            Controller.Url = urlHelperMock.Object;

            Controller.Post(board);

            BoardManagerMock.Verify(x => x.Add(It.Is<Board>(b => b.CreatorId == userId)), Times.Once);
        }
    }
}
