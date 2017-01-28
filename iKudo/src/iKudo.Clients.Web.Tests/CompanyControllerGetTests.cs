using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class CompanyControllerGetTests
    {
        Mock<ICompanyManager> companyManagerMock = new Mock<ICompanyManager>();

        [Fact]
        public void CompanyGet_Returns_Ok_With_Company_Object()
        {
            int companyId = 33;
            Company company = new Company { Name = "name", Description = "desc", CreatorId = "DE%$EDS" };
            companyManagerMock.Setup(x => x.GetCompany(It.Is<int>(c=>c == companyId))).Returns(company);
            CompanyController controller = new CompanyController(companyManagerMock.Object);

            OkObjectResult response = controller.Get(companyId) as OkObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
            Assert.True(response.Value is Company);
        }

        [Fact]
        public void CompanyGet_Calls_GetCompany_Once()
        {
            int companyId = 33;
            CompanyController controller = new CompanyController(companyManagerMock.Object);

            controller.Get(companyId);

            companyManagerMock.Verify(x => x.GetCompany(companyId), Times.Once);
        }

        [Fact]
        public void CompanyGet_Returns_NotFound_If_CompanyId_Not_Exist()
        {
            int companyId = 33;
            companyManagerMock.Setup(x => x.GetCompany(It.Is<int>(c => c == companyId))).Returns((Company)null);
            CompanyController controller = new CompanyController(companyManagerMock.Object);

            NotFoundResult response = controller.Get(companyId) as NotFoundResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Company_Get_Returns_Error_On_Unknown_Exception()
        {
            string exceptionMessage = "Nieoczekiwany błąd";
            companyManagerMock.Setup(x => x.GetCompany(It.IsAny<int>()))
                              .Throws(new Exception(exceptionMessage));
            CompanyController controller = new CompanyController(companyManagerMock.Object);

            int companyId = 45;
            ObjectResult response = controller.Get(companyId) as ObjectResult;

            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(exceptionMessage, response.Value.ToString());
        }
    }
}
