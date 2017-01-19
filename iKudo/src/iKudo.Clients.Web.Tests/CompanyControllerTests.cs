using iKudo.Controllers.Api;
using System;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class CompanyControllerTests
    {
        private readonly CompanyController companyController;

        public CompanyControllerTests()
        {
            companyController = new CompanyController();
        }

        [Fact]
        public void MyMethod()
        {
            companyController.MyMethod();
            Assert.True(true);
        }
    }
}
