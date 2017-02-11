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
        Mock<ICompanyManager> companyManagerMock = new Mock<ICompanyManager>();
        Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();

        public CompanyControllerPostTests()
        {
            urlHelperMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(locationUrl);
        }

        [Fact]
        public void Company_Post_Returns_Created_Status()
        {
            var companyController = new CompanyController(companyManagerMock.Object);
            companyController.Url = urlHelperMock.Object;
            Company company = new Company();

            CreatedResult response = companyController.Post(company) as CreatedResult;

            Assert.True(response.StatusCode == (int)HttpStatusCode.Created);
        }

        [Fact]
        public void Company_Post_Returns_Location()
        {
            var companyController = new CompanyController(companyManagerMock.Object);
            companyController.Url = urlHelperMock.Object;
            Company company = new Company();

            CreatedResult response = companyController.Post(company) as CreatedResult;

            Assert.Equal(locationUrl, response.Location);
        }

        [Fact]
        public void Company_Post_Calls_Once_InsertCompany()
        {
            CompanyController controller = new CompanyController(companyManagerMock.Object);
            controller.Url = urlHelperMock.Object;

            Company company = new Company();

            controller.Post(company);

            companyManagerMock.Verify(x => x.InsertCompany(It.IsAny<Company>()), Times.Once);
        }

        [Fact]
        public void Company_Post_Returns_Errors_If_Model_Is_Invalid()
        {
            CompanyController controller = new CompanyController(companyManagerMock.Object);
            controller.ModelState.AddModelError("property", "error");
            Company company = new Company();

            BadRequestObjectResult response = controller.Post(company) as BadRequestObjectResult;

            Assert.Equal((int)HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public void Company_Post_Returns_ConflictResult_If_Name_Exists()
        {
            string exceptionMessage = "Obiekt już istnieje";
            companyManagerMock.Setup(x => x.InsertCompany(It.IsAny<Company>()))
                              .Throws(new CompanyAlreadyExistException(exceptionMessage));
            CompanyController controller = new CompanyController(companyManagerMock.Object);
            Company company = new Company() { Name = "existing name" };

            ObjectResult response = controller.Post(company) as ObjectResult;
            
            Assert.Equal(HttpStatusCode.Conflict, (HttpStatusCode)response.StatusCode);

            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void Company_Post_Returns_Error_On_Unknown_Exception()
        {
            string exceptionMessage = "Nieoczekiwany błąd";
            companyManagerMock.Setup(x => x.InsertCompany(It.IsAny<Company>()))
                              .Throws(new Exception(exceptionMessage));
            CompanyController controller = new CompanyController(companyManagerMock.Object);
            Company company = new Company() { Name = "existing name" };

            ObjectResult response = controller.Post(company) as ObjectResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);

            string error = response.Value.GetType().GetProperty("Error").GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }
    }
}
