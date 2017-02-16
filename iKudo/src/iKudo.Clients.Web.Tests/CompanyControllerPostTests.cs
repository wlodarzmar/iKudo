using iKudo.Controllers.Api;
using iKudo.Domain;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Reflection;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class CompanyControllerPostTests
    {
        private string locationUrl = "http://location/";
        Mock<IGroupManager> companyManagerMock = new Mock<IGroupManager>();
        Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();

        public CompanyControllerPostTests()
        {
            urlHelperMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(locationUrl);
        }

        [Fact]
        public void Company_Post_Returns_Created_Status()
        {
            var companyController = new GroupController(companyManagerMock.Object);
            companyController.Url = urlHelperMock.Object;
            Group company = new Group();

            CreatedResult response = companyController.Post(company) as CreatedResult;

            Assert.True(response.StatusCode == (int)HttpStatusCode.Created);
        }

        [Fact]
        public void Company_Post_Returns_Location()
        {
            var companyController = new GroupController(companyManagerMock.Object);
            companyController.Url = urlHelperMock.Object;
            Group company = new Group();

            CreatedResult response = companyController.Post(company) as CreatedResult;

            Assert.Equal(locationUrl, response.Location);
        }

        [Fact]
        public void Company_Post_Calls_Once_InsertCompany()
        {
            GroupController controller = new GroupController(companyManagerMock.Object);
            controller.Url = urlHelperMock.Object;

            Group company = new Group();

            controller.Post(company);

            companyManagerMock.Verify(x => x.Add(It.IsAny<Group>()), Times.Once);
        }

        [Fact]
        public void Company_Post_Returns_Errors_If_Model_Is_Invalid()
        {
            GroupController controller = new GroupController(companyManagerMock.Object);
            controller.ModelState.AddModelError("property", "error");
            Group company = new Group();

            BadRequestObjectResult response = controller.Post(company) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public void Company_Post_Returns_ConflictResult_If_Name_Exists()
        {
            string exceptionMessage = "Obiekt już istnieje";
            companyManagerMock.Setup(x => x.Add(It.IsAny<Group>()))
                              .Throws(new CompanyAlreadyExistException(exceptionMessage));
            GroupController controller = new GroupController(companyManagerMock.Object);
            Group company = new Group() { Name = "existing name" };

            ObjectResult response = controller.Post(company) as ObjectResult;
            
            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)response.StatusCode);

            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void Company_Post_Returns_Error_On_Unknown_Exception()
        {
            string exceptionMessage = "Nieoczekiwany błąd";
            companyManagerMock.Setup(x => x.Add(It.IsAny<Group>()))
                              .Throws(new Exception(exceptionMessage));
            GroupController controller = new GroupController(companyManagerMock.Object);
            Group company = new Group() { Name = "existing name" };

            ObjectResult response = controller.Post(company) as ObjectResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);

            string error = response.Value.GetType().GetProperty("Error").GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }
    }
}
