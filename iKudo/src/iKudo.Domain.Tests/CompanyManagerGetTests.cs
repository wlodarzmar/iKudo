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
        IQueryable<Group> data = new List<Group> {
            new Group { Id = 1, Name = "company name" },
            new Group { Id = 2, Name = "company name 2" }
        }.AsQueryable();

        Mock<DbSet<Group>> companiesMock = new Mock<DbSet<Group>>();
        Mock<KudoDbContext> kudoDbContextMock = new Mock<KudoDbContext>();

        public CompanyManagerGetTests()
        {
            companiesMock = ConfigureCompaniesMock(data);
            kudoDbContextMock.Setup(x => x.Groups).Returns(companiesMock.Object);
        }

        [Fact]
        public void CompanyManager_GetCompany_Returns_Null_If_Not_Found_Company()
        {
            IGroupManager manager = new GroupManager(kudoDbContextMock.Object);

            int companyId = 3;
            Group company = manager.Get(companyId);

            Assert.Null(company);
        }

        [Fact]
        public void CompanyManager_GetCompany_Returns_Company()
        {
            IGroupManager manager = new GroupManager(kudoDbContextMock.Object);

            int companyId = 1;
            Group company = manager.Get(companyId);

            Assert.NotNull(company);
        }

        [Fact]
        public void CompanyManager_GetAll_ReturnsAllCompanies()
        {
            IGroupManager manager = new GroupManager(kudoDbContextMock.Object);

            ICollection<Group> companies = manager.GetAll();

            Assert.NotNull(companies);
            Assert.Equal(data.Count(), companies.Count);
        }
    }
}
