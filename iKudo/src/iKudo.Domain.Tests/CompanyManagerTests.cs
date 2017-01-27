using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class CompanyManagerTests
    {
        [Fact]
        public void CompanyManager_Throws_ArgumentNullException_If_Company_Is_Null()
        {
            var kudoDbContextMock = new Mock<KudoDbContext>();
            CompanyManager manager = new CompanyManager(kudoDbContextMock.Object);

            Assert.Throws(typeof(ArgumentNullException), () => manager.InsertCompany(null));
        }

        [Fact]
        public void CompanyManager_InsertCompany_Calls_Companies_Add_Once()
        {
            var data = new List<Company> { new Company { Name = "name" } }.AsQueryable();
            var companiesMock = new Mock<DbSet<Company>>();
            companiesMock.As<IQueryable<Company>>().Setup(x => x.Provider).Returns(data.Provider);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.Expression).Returns(data.Expression);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.ElementType).Returns(data.ElementType);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
            var kudoDbContextMock = new Mock<KudoDbContext>();
            kudoDbContextMock.Setup(x => x.Companies).Returns(companiesMock.Object);
            CompanyManager manager = new CompanyManager(kudoDbContextMock.Object);

            Company company = new Company() { Name = "company name" };
            manager.InsertCompany(company);

            companiesMock.Verify(x => x.Add(It.Is<Company>(c => c.Name == company.Name)), Times.Once);
            kudoDbContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CompanyManager_InsertCompany_Returns_AddedCompany()
        {
            var data = new List<Company> { new Company { Name = "name" } }.AsQueryable();
            var companiesMock = new Mock<DbSet<Company>>();
            companiesMock.As<IQueryable<Company>>().Setup(x => x.Provider).Returns(data.Provider);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.Expression).Returns(data.Expression);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.ElementType).Returns(data.ElementType);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
            var kudoDbContextMock = new Mock<KudoDbContext>();
            kudoDbContextMock.Setup(x => x.Companies).Returns(companiesMock.Object);
            CompanyManager manager = new CompanyManager(kudoDbContextMock.Object);

            Company company = new Company() { Name = "company name" };
            Company addedCompany = manager.InsertCompany(company);

            Assert.NotNull(addedCompany);
            Assert.Equal(company.Name, addedCompany.Name);
        }

        [Fact]
        public void CompanyManager_InsertCompany_Throws_Exception_If_Company_Name_Exists()
        {
            var data = new List<Company> { new Company { Name = "company name" } }.AsQueryable();
            var companiesMock = new Mock<DbSet<Company>>();
            companiesMock.As<IQueryable<Company>>().Setup(x => x.Provider).Returns(data.Provider);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.Expression).Returns(data.Expression);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.ElementType).Returns(data.ElementType);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
            var kudoDbContextMock = new Mock<KudoDbContext>();
            kudoDbContextMock.Setup(x => x.Companies).Returns(companiesMock.Object);
            CompanyManager manager = new CompanyManager(kudoDbContextMock.Object);

            Company company = new Company() { Name = "company name" };

            Assert.Throws<CompanyAlreadyExistException>(() =>
            {
                manager.InsertCompany(company);
            });
        }
    }
}
