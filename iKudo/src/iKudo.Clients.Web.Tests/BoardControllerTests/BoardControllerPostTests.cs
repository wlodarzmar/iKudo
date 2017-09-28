using FluentAssertions;
using iKudo.Clients.Web.Filters;
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
    public class BoardControllerPostTests
    {
        private string locationUrl = "http://location/";
        private Mock<IManageBoards> boardManagerMock;
        private Mock<IUrlHelper> urlHelperMock;
        private Mock<IDtoFactory> dtoFactoryMock;

        public BoardControllerPostTests()
        {
            dtoFactoryMock = new Mock<IDtoFactory>();
            boardManagerMock = new Mock<IManageBoards>();
            urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(locationUrl);
        }

        [Fact]
        public void Post_ValidRequest_ReturnsCreatedStatus()
        {
            BoardController boardController = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            boardController.WithCurrentUser();
            boardController.Url = urlHelperMock.Object;
            BoardDTO board = new BoardDTO();

            CreatedResult response = boardController.Post(board) as CreatedResult;

            Assert.True(response.StatusCode == (int)HttpStatusCode.Created);
        }

        [Fact]
        public void Post_ValidRequest_ReturnsLocation()
        {
            BoardController boardController = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            boardController.Url = urlHelperMock.Object;
            boardController.WithCurrentUser();
            BoardDTO board = new BoardDTO();

            CreatedResult response = boardController.Post(board) as CreatedResult;

            Assert.Equal(locationUrl, response.Location);
        }

        [Fact]
        public void Post_ValidRequest_CallsAddOnce()
        {
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            controller.Url = urlHelperMock.Object;

            BoardDTO board = new BoardDTO();

            controller.Post(board);

            boardManagerMock.Verify(x => x.Add(It.IsAny<Board>()), Times.Once);
        }

        [Fact]
        public void Post_NameAlreadyExists_ReturnsConflict()
        {
            string exceptionMessage = "Obiekt już istnieje";
            boardManagerMock.Setup(x => x.Add(It.IsAny<Board>()))
                              .Throws(new AlreadyExistException(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            BoardDTO board = new BoardDTO() { Name = "existing name" };

            ObjectResult response = controller.Post(board) as ObjectResult;

            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }

        [Fact]
        public void Post_ThrowsUnknownException_ReturnsInternalServerError()
        {
            string exceptionMessage = "Nieoczekiwany błąd";
            boardManagerMock.Setup(x => x.Add(It.IsAny<Board>()))
                            .Throws(new Exception(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser();
            BoardDTO board = new BoardDTO() { Name = "name" };

            ObjectResult response = controller.Post(board) as ObjectResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Error);
        }

        [Fact]
        public void Post_CallsAddWithCurrentUserIdAsCreator()
        {
            string userId = "userId";
            BoardDTO board = new BoardDTO { Name = "name" };
            dtoFactoryMock.Setup(x => x.Create<Board, BoardDTO>(It.IsAny<BoardDTO>())).Returns(new Board { Name = board.Name, CreatorId = userId });
            BoardController controller = new BoardController(boardManagerMock.Object, dtoFactoryMock.Object);
            controller.WithCurrentUser(userId);
            controller.Url = urlHelperMock.Object;

            controller.Post(board);

            boardManagerMock.Verify(x => x.Add(It.Is<Board>(b => b.CreatorId == userId)), Times.Once);
        }
    }
}
