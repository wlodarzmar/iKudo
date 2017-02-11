using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class CompanyManagerGetTests : CompanyTestsBase
    {
        IQueryable<Company> data = new List<Company> {
            new Company { Id = 1, Name = "company name" },
            new Company { Id = 2, Name = "company name 2" }
        }.AsQueryable();

        Mock<DbSet<Company>> companiesMock = new Mock<DbSet<Company>>();
        Mock<KudoDbContext> kudoDbContextMock = new Mock<KudoDbContext>();

        public CompanyManagerGetTests()
        {
            companiesMock = ConfigureCompaniesMock(data);
            kudoDbContextMock.Setup(x => x.Companies).Returns(companiesMock.Object);
        }

        [Fact]
        public void CompanyManager_GetCompany_Returns_Null_If_Not_Found_Company()
        {
            ICompanyManager manager = new CompanyManager(kudoDbContextMock.Object);

            int companyId = 3;
            Company company = manager.GetCompany(companyId);

            Assert.Null(company);
        }

        [Fact]
        public void CompanyManager_GetCompany_Returns_Company()
        {
            ICompanyManager manager = new CompanyManager(kudoDbContextMock.Object);

            int companyId = 1;
            Company company = manager.GetCompany(companyId);

            Assert.NotNull(company);
        }

        [Fact]
        public void CompanyManager_GetAll_ReturnsAllCompanies()
        {
            ICompanyManager manager = new CompanyManager(kudoDbContextMock.Object);

            ICollection<Company> companies = manager.GetAll();

            Assert.NotNull(companies);
            Assert.Equal(data.Count(), companies.Count);
        }
    }
}
