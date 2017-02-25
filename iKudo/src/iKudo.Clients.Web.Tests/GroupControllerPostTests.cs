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
    public class GroupControllerPostTests
    {
        private string locationUrl = "http://location/";
        Mock<IGroupManager> groupManagerMock = new Mock<IGroupManager>();
        Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();

        public GroupControllerPostTests()
        {
            urlHelperMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(locationUrl);
        }

        [Fact]
        public void Group_Post_Returns_Created_Status()
        {
            var groupController = new GroupController(groupManagerMock.Object);
            groupController.Url = urlHelperMock.Object;
            Group group = new Group();

            CreatedResult response = groupController.Post(group) as CreatedResult;

            Assert.True(response.StatusCode == (int)HttpStatusCode.Created);
        }

        [Fact]
        public void Group_Post_Returns_Location()
        {
            var groupController = new GroupController(groupManagerMock.Object);
            groupController.Url = urlHelperMock.Object;
            Group group = new Group();

            CreatedResult response = groupController.Post(group) as CreatedResult;

            Assert.Equal(locationUrl, response.Location);
        }

        [Fact]
        public void Group_Post_Calls_Once_InsertGroup()
        {
            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.Url = urlHelperMock.Object;

            Group group = new Group();

            controller.Post(group);

            groupManagerMock.Verify(x => x.Add(It.IsAny<Group>()), Times.Once);
        }

        [Fact]
        public void Group_Post_Returns_Errors_If_Model_Is_Invalid()
        {
            GroupController controller = new GroupController(groupManagerMock.Object);
            controller.ModelState.AddModelError("property", "error");
            Group group = new Group();

            BadRequestObjectResult response = controller.Post(group) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public void Group_Post_Returns_ConflictResult_If_Name_Exists()
        {
            string exceptionMessage = "Obiekt już istnieje";
            groupManagerMock.Setup(x => x.Add(It.IsAny<Group>()))
                              .Throws(new GroupAlreadyExistException(exceptionMessage));
            GroupController controller = new GroupController(groupManagerMock.Object);
            Group group = new Group() { Name = "existing name" };

            ObjectResult response = controller.Post(group) as ObjectResult;
            
            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)response.StatusCode);

            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void Group_Post_Returns_Error_On_Unknown_Exception()
        {
            string exceptionMessage = "Nieoczekiwany błąd";
            groupManagerMock.Setup(x => x.Add(It.IsAny<Group>()))
                              .Throws(new Exception(exceptionMessage));
            GroupController controller = new GroupController(groupManagerMock.Object);
            Group group = new Group() { Name = "existing name" };

            ObjectResult response = controller.Post(group) as ObjectResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);

            string error = response.Value.GetType().GetProperty("Error").GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }
    }
}
