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
    public class BoardControllerPostTests
    {
        private string locationUrl = "http://location/";
        Mock<IBoardManager> boardManagerMock = new Mock<IBoardManager>();
        Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();

        public BoardControllerPostTests()
        {
            urlHelperMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(locationUrl);
        }

        [Fact]
        public void Board_Post_Returns_Created_Status()
        {
            var boardController = new BoardController(boardManagerMock.Object);
            boardController.Url = urlHelperMock.Object;
            Board board = new Board();

            CreatedResult response = boardController.Post(board) as CreatedResult;

            Assert.True(response.StatusCode == (int)HttpStatusCode.Created);
        }

        [Fact]
        public void Board_Post_Returns_Location()
        {
            var boardController = new BoardController(boardManagerMock.Object);
            boardController.Url = urlHelperMock.Object;
            Board board = new Board();

            CreatedResult response = boardController.Post(board) as CreatedResult;

            Assert.Equal(locationUrl, response.Location);
        }

        [Fact]
        public void Board_Post_Calls_Once_InsertBoard()
        {
            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.Url = urlHelperMock.Object;

            Board board = new Board();

            controller.Post(board);

            boardManagerMock.Verify(x => x.Add(It.IsAny<Board>()), Times.Once);
        }

        [Fact]
        public void Board_Post_Returns_Errors_If_Model_Is_Invalid()
        {
            BoardController controller = new BoardController(boardManagerMock.Object);
            controller.ModelState.AddModelError("property", "error");
            Board board = new Board();

            BadRequestObjectResult response = controller.Post(board) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public void Board_Post_Returns_ConflictResult_If_Name_Exists()
        {
            string exceptionMessage = "Obiekt już istnieje";
            boardManagerMock.Setup(x => x.Add(It.IsAny<Board>()))
                              .Throws(new AlreadyExistException(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object);
            Board board = new Board() { Name = "existing name" };

            ObjectResult response = controller.Post(board) as ObjectResult;
            
            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)response.StatusCode);

            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void Board_Post_Returns_Error_On_Unknown_Exception()
        {
            string exceptionMessage = "Nieoczekiwany błąd";
            boardManagerMock.Setup(x => x.Add(It.IsAny<Board>()))
                              .Throws(new Exception(exceptionMessage));
            BoardController controller = new BoardController(boardManagerMock.Object);
            Board board = new Board() { Name = "existing name" };

            ObjectResult response = controller.Post(board) as ObjectResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);

            string error = response.Value.GetType().GetProperty("Error").GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }
    }
}
