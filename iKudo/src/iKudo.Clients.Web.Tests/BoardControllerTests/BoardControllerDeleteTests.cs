using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class BoardControllerDeleteTests : BoardControllerTestsBase
    {
        [Fact]
        public void Delete_ValidRequest_ReturnsNoContent()
        {
            StatusCodeResult response = Controller.Delete(1) as StatusCodeResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NoContent, (HttpStatusCode)response.StatusCode);
        }
        
        [Fact]
        public void Delete_ValidRequest_CallsManagerDeleteOnceWithProperArguments()
        {
            string userId = "adadqwee123";
            Controller.WithCurrentUser(userId);

            int idToDelete = 1;

            Controller.Delete(idToDelete);

            BoardManagerMock.Verify(x => x.Delete(It.Is<string>(i => i == userId), It.Is<int>(i => i == idToDelete)), Times.Once);
        }

        [Fact]
        public void Delete_BoardDoesntExist_ReturnsNotFound()
        {
            string exceptionMessage = "exception message";
            BoardManagerMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<int>())).Throws(new NotFoundException(exceptionMessage));
            
            ObjectResult response = Controller.Delete(12) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Message);
        }
        
        [Fact]
        public void Delete_UserTriesDeleteForeignBoard_ReturnsForbiden()
        {
            string exceptionMessage = "exception message";
            BoardManagerMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<int>())).Throws(new UnauthorizedAccessException(exceptionMessage));

            ObjectResult response = Controller.Delete(1) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Forbidden, (HttpStatusCode)response.StatusCode);
            Assert.Equal(exceptionMessage, (response.Value as ErrorResult).Message);
        }
    }
}
