using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class GroupControllerDeleteTests
    {
        private Mock<IGroupManager> groupManagerMock = new Mock<IGroupManager>();


        [Fact]
        public void Group_Delete_Returns_Ok()
        {
            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.ControllerContext = GetHttpContext();
            StatusCodeResult response = controller.Delete(1) as StatusCodeResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Group_Delete_Calls_ManagerDelete_Once_With_Proper_Arguments()
        {
            GroupController controller = new GroupController(groupManagerMock.Object);
            string userId = "adadqwee123";
            controller.ControllerContext = GetHttpContext(userId);

            int idToDelete = 1;

            controller.Delete(idToDelete);

            groupManagerMock.Verify(x => x.Delete(It.Is<string>(i => i == userId), It.Is<int>(i => i == idToDelete)), Times.Once);
        }

        [Fact]
        public void Group_Delete_Returns_NotFound_If_Group_Not_Exist()
        {
            string exceptionMessage = "exception message";
            groupManagerMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<int>())).Throws(new NotFoundException(exceptionMessage));
            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.ControllerContext = GetHttpContext();

            ObjectResult response = controller.Delete(12) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void Group_Delete_Returns_ServerError_When_Exception()
        {
            string exceptionMessage = "exception msg";
            groupManagerMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<int>())).Throws(new Exception(exceptionMessage));
            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.ControllerContext = GetHttpContext();

            ObjectResult response = controller.Delete(1) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void Group_Delete_Returns_Forbiden_If_Try_Delete_Foreign_Group()
        {
            string exceptionMessage = "exception message";
            groupManagerMock.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<int>())).Throws(new UnauthorizedAccessException(exceptionMessage));
            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.ControllerContext = GetHttpContext();

            ObjectResult response = controller.Delete(1) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Forbidden, (HttpStatusCode)response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        private ControllerContext GetHttpContext(string userId = null)
        {
            List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId ?? "") };
            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }
    }
}
