using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class CompanyControllerGetTests
    {
        Mock<IGroupManager> companyManagerMock = new Mock<IGroupManager>();

        [Fact]
        public void CompanyGet_Returns_Ok_With_Company_Object()
        {
            int companyId = 33;
            Group company = new Group { Name = "name", Description = "desc", CreatorId = "DE%$EDS" };
            companyManagerMock.Setup(x => x.Get(It.Is<int>(c => c == companyId))).Returns(company);
            GroupController controller = new GroupController(companyManagerMock.Object);

            OkObjectResult response = controller.Get(companyId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
            Assert.True(response.Value is Group);
        }

        [Fact]
        public void CompanyGet_Calls_GetCompany_Once()
        {
            int companyId = 33;
            GroupController controller = new GroupController(companyManagerMock.Object);

            controller.Get(companyId);

            companyManagerMock.Verify(x => x.Get(companyId), Times.Once);
        }

        [Fact]
        public void CompanyGet_Returns_NotFound_If_CompanyId_Not_Exist()
        {
            int companyId = 33;
            companyManagerMock.Setup(x => x.Get(It.Is<int>(c => c == companyId))).Returns((Group)null);
            GroupController controller = new GroupController(companyManagerMock.Object);

            NotFoundResult response = controller.Get(companyId) as NotFoundResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Company_Get_Returns_Error_On_Unknown_Exception()
        {
            string exceptionMessage = "Nieoczekiwany błąd";
            companyManagerMock.Setup(x => x.Get(It.IsAny<int>()))
                              .Throws(new Exception(exceptionMessage));
            GroupController controller = new GroupController(companyManagerMock.Object);

            int companyId = 45;
            ObjectResult response = controller.Get(companyId) as ObjectResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(exceptionMessage, response.Value.ToString());
        }

        [Fact]
        public void Company_GetAll_Returns_All_Companies()
        {
            ICollection<Group> data = new List<Group> {
                new Group { Id = 1, Name = "company name" },
                new Group { Id = 2, Name = "company name 2" },
                new Group { Id = 3, Name = "company name 3" }
            };
            companyManagerMock.Setup(x => x.GetAll()).Returns(data);

            GroupController controller = new GroupController(companyManagerMock.Object);

            OkObjectResult response = controller.GetAll() as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
            List<Group> companies = response.Value as List<Group>;
            Assert.Equal(data.Count, companies.Count);
        }

        [Fact]
        public void Company_GetAll_Returns_Error_If_Unknown_Exception()
        {
            string exceptionMessage = "Wystąpił błąd";
            companyManagerMock.Setup(x => x.GetAll()).Throws(new Exception(exceptionMessage));
            GroupController controller = new GroupController(companyManagerMock.Object);

            ObjectResult response = controller.GetAll() as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(exceptionMessage, response.Value.ToString());
        }

        [Fact]
        public void Company_GetAll_Calls_CompanyManagerGetAll_Once()
        {
            GroupController controller = new GroupController(companyManagerMock.Object);

            controller.GetAll();

            companyManagerMock.Verify(x => x.GetAll(), Times.Once);
        }
    }
}
