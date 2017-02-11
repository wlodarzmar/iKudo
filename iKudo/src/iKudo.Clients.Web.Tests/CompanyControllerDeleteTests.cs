using iKudo.Controllers.Api;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using System.Reflection;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class CompanyControllerDeleteTests
    {
        private Mock<ICompanyManager> companyManagerMock = new Mock<ICompanyManager>();

        [Fact]
        public void Company_Delete_Returns_Ok()
        {
            CompanyController controller = new CompanyController(companyManagerMock.Object);

            StatusCodeResult response = controller.Delete(1) as StatusCodeResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)response.StatusCode);
        }

        [Fact]
        public void Company_Delete_Calls_ManagerDelete_Once()
        {
            CompanyController controller = new CompanyController(companyManagerMock.Object);

            int idToDelete = 1;
            controller.Delete(idToDelete);

            companyManagerMock.Verify(x => x.Delete(It.Is<int>(i => i == idToDelete)), Times.Once);
        }

        [Fact]
        public void Company_Delete_Returns_NotFound_If_Company_Not_Exist()
        {
            string exceptionMessage = "exception message";
            companyManagerMock.Setup(x => x.Delete(It.IsAny<int>())).Throws(new NotFoundException(exceptionMessage));
            CompanyController controller = new CompanyController(companyManagerMock.Object);

            ObjectResult response = controller.Delete(12) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }

        [Fact]
        public void Company_Delete_Returns_ServerError_When_Exception()
        {
            string exceptionMessage = "exception msg";
            companyManagerMock.Setup(x => x.Delete(It.IsAny<int>())).Throws(new Exception(exceptionMessage));
            CompanyController controller = new CompanyController(companyManagerMock.Object);

            ObjectResult response = controller.Delete(1) as ObjectResult;

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)response.StatusCode);
            string error = response?.Value?.GetType()?.GetProperty("Error")?.GetValue(response.Value) as string;
            Assert.Equal(exceptionMessage, error);
        }
    }
}
