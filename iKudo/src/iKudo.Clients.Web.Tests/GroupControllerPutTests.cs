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
    public class GroupControllerPutTests : GroupControllerTestBase
    {
        Mock<IGroupManager> groupManagerMock = new Mock<IGroupManager>();

        [Fact]
        public void GroupController_Put_Returns_Ok_After_Succeeded_Modification()
        {
            Group group = new Group { Id = 1, Name = "name", CreationDate = DateTime.Now };

            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.ControllerContext = GetHttpContext(null);
            OkResult response = controller.Put(group) as OkResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void GroupController_Put_Calls_GroupManager_Update_Once()
        {
            Group group = new Group { Id = 1, Name = "name", CreationDate = DateTime.Now };
            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.ControllerContext = GetHttpContext(null);

            controller.Put(group);

            groupManagerMock.Verify(x => x.Update(It.Is<Group>(g => g == group)), Times.Once);
        }

        [Fact]
        public void GroupController_Put_Returns_Conflict_If_Name_Exist()
        {
            Group group = new Group { Id = 1, Name = "name", CreationDate = DateTime.Now };
            string exceptionMessage = "group already exist";
            groupManagerMock.Setup(x => x.Update(It.IsAny<Group>())).Throws(new GroupAlreadyExistException(exceptionMessage));

            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.ControllerContext = GetHttpContext(null);

            ObjectResult response = controller.Put(group) as ObjectResult;

            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void GroupController_Put_Returns_BadRequest_If_Model_Is_Invalid()
        {
            Group group = new Group();
            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.ControllerContext = GetHttpContext(null);            
            controller.ModelState.AddModelError("property", "error");

            BadRequestObjectResult response = controller.Put(group) as BadRequestObjectResult;

            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void GroupController_Returns_ServerError_If_Exception()
        {
            string exceptionMessage = "group already exist";
            groupManagerMock.Setup(x => x.Update(It.IsAny<Group>())).Throws(new Exception(exceptionMessage));

            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.ControllerContext = GetHttpContext(null);

            ObjectResult response = controller.Put(new Group()) as ObjectResult;

            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void GroupController_Put_Returns_NotFound_If_Group_Not_Exist()
        {
            Group group = new Group { Id = 45345, Name = "name", CreationDate = DateTime.Now };
            string exceptionMessage = "group does not exist";
            groupManagerMock.Setup(x => x.Update(It.IsAny<Group>())).Throws(new NotFoundException(exceptionMessage));

            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.ControllerContext = GetHttpContext(null);

            NotFoundResult response = controller.Put(group) as NotFoundResult;

            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void GroupController_Returns_Forbidden_If_Edit_Foreign_Group()
        {
            Group group = new Group {CreatorId = "creatorId", Id = 45345, Name = "name", CreationDate = DateTime.Now };
            string exceptionMessage = "unauthorized exception message";
            groupManagerMock.Setup(x => x.Update(It.IsAny<Group>())).Throws(new UnauthorizedAccessException(exceptionMessage));

            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.ControllerContext = GetHttpContext();
            ObjectResult response = controller.Put(group) as ObjectResult;

            Assert.Equal(HttpStatusCode.Forbidden, (HttpStatusCode)response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }
    }
}
