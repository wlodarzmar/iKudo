using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class CompanyManagerGetTests
    {
        IQueryable<Company> data = new List<Company> {
            new Company { Id = 1, Name = "company name" },
            new Company { Id = 2, Name = "company name 2" }
        }.AsQueryable();

        Mock<DbSet<Company>> companiesMock = new Mock<DbSet<Company>>();
        Mock<KudoDbContext> kudoDbContextMock = new Mock<KudoDbContext>();

        public CompanyManagerGetTests()
        {
            companiesMock.As<IQueryable<Company>>().Setup(x => x.Provider).Returns(data.Provider);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.Expression).Returns(data.Expression);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.ElementType).Returns(data.ElementType);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
            kudoDbContextMock.Setup(x => x.Companies).Returns(companiesMock.Object);
        }

        [Fact]
        public void CompanyManager_GetCompany_Returns_Null_If_Not_Found_Company()
        {
            CompanyManager manger = new CompanyManager(kudoDbContextMock.Object);

            int companyId = 3;
            Company company = manger.GetCompany(companyId);

            Assert.Null(company);
        }

        [Fact]
        public void CompanyManager_GetCompany_Returns_Company()
        {
            CompanyManager manger = new CompanyManager(kudoDbContextMock.Object);

            int companyId = 1;
            Company company = manger.GetCompany(companyId);

            Assert.NotNull(company);
        }
    }
}
