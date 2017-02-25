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
    public class GroupControllerGetTests
    {
        Mock<IGroupManager> groupManagerMock = new Mock<IGroupManager>();

        [Fact]
        public void GroupGet_Returns_Ok_With_Group_Object()
        {
            int groupId = 33;
            Group group = new Group { Name = "name", Description = "desc", CreatorId = "DE%$EDS" };
            groupManagerMock.Setup(x => x.Get(It.Is<int>(c => c == groupId))).Returns(group);
            GroupController controller = new GroupController(groupManagerMock.Object);

            OkObjectResult response = controller.Get(groupId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
            Assert.True(response.Value is Group);
        }

        [Fact]
        public void GroupGet_Calls_GetGroup_Once()
        {
            int groupId = 33;
            GroupController controller = new GroupController(groupManagerMock.Object);

            controller.Get(groupId);

            groupManagerMock.Verify(x => x.Get(groupId), Times.Once);
        }

        [Fact]
        public void GroupGet_Returns_NotFound_If_GroupId_Not_Exist()
        {
            int groupId = 33;
            groupManagerMock.Setup(x => x.Get(It.Is<int>(c => c == groupId))).Returns((Group)null);
            GroupController controller = new GroupController(groupManagerMock.Object);

            NotFoundResult response = controller.Get(groupId) as NotFoundResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Group_Get_Returns_Error_On_Unknown_Exception()
        {
            string exceptionMessage = "Nieoczekiwany błąd";
            groupManagerMock.Setup(x => x.Get(It.IsAny<int>()))
                              .Throws(new Exception(exceptionMessage));
            GroupController controller = new GroupController(groupManagerMock.Object);

            int groupId = 45;
            ObjectResult response = controller.Get(groupId) as ObjectResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(exceptionMessage, response.Value.ToString());
        }

        [Fact]
        public void Group_GetAll_Returns_All_Companies()
        {
            ICollection<Group> data = new List<Group> {
                new Group { Id = 1, Name = "group name" },
                new Group { Id = 2, Name = "group name 2" },
                new Group { Id = 3, Name = "group name 3" }
            };
            groupManagerMock.Setup(x => x.GetAll()).Returns(data);

            GroupController controller = new GroupController(groupManagerMock.Object);

            OkObjectResult response = controller.GetAll() as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            List<Group> companies = response.Value as List<Group>;
            Assert.Equal(data.Count, companies.Count);
        }

        [Fact]
        public void Group_GetAll_Returns_Error_If_Unknown_Exception()
        {
            string exceptionMessage = "Wystąpił błąd";
            groupManagerMock.Setup(x => x.GetAll()).Throws(new Exception(exceptionMessage));
            GroupController controller = new GroupController(groupManagerMock.Object);

            ObjectResult response = controller.GetAll() as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void Group_GetAll_Calls_GroupManagerGetAll_Once()
        {
            GroupController controller = new GroupController(groupManagerMock.Object);

            controller.GetAll();

            groupManagerMock.Verify(x => x.GetAll(), Times.Once);
        }
    }
}
