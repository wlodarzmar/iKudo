using iKudo.Domain.Exceptions;
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
    public class CompanyManagerInsertTests :CompanyTestsBase
    {
        [Fact]
        public void CompanyManager_Throws_ArgumentNullException_If_Company_Is_Null()
        {
            var kudoDbContextMock = new Mock<KudoDbContext>();
            IGroupManager manager = new GroupManager(kudoDbContextMock.Object);

            Assert.Throws(typeof(ArgumentNullException), () => manager.Add(null));
        }

        [Fact]
        public void CompanyManager_InsertCompany_Calls_Companies_Add_Once()
        {
            var data = new List<Group> { new Group { Name = "name" } }.AsQueryable();
            var companiesMock = ConfigureCompaniesMock(data);
            var kudoDbContextMock = new Mock<KudoDbContext>();
            kudoDbContextMock.Setup(x => x.Groups).Returns(companiesMock.Object);
            IGroupManager manager = new GroupManager(kudoDbContextMock.Object);

            Group company = new Group() { Name = "company name" };
            manager.Add(company);

            companiesMock.Verify(x => x.Add(It.Is<Group>(c => c.Name == company.Name)), Times.Once);
            kudoDbContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CompanyManager_InsertCompany_Returns_AddedCompany()
        {
            DateTime now = DateTime.Now;
            var data = new List<Group> { new Group { Name = "name", } }.AsQueryable();
            var companiesMock = ConfigureCompaniesMock(data);
            var kudoDbContextMock = new Mock<KudoDbContext>();
            kudoDbContextMock.Setup(x => x.Groups).Returns(companiesMock.Object);
            IGroupManager manager = new GroupManager(kudoDbContextMock.Object);

            Group company = new Group() { Name = "company name" };
            Group addedCompany = manager.Add(company);

            Assert.NotNull(addedCompany);
            Assert.Equal(company.Name, addedCompany.Name);
            Assert.True(company.CreationDate >= now);
        }

        [Fact]
        public void CompanyManager_InsertCompany_Throws_Exception_If_Company_Name_Exists()
        {
            var data = new List<Group> { new Group { Name = "company name" } }.AsQueryable();
            var companiesMock = ConfigureCompaniesMock(data);
            var kudoDbContextMock = new Mock<KudoDbContext>();
            kudoDbContextMock.Setup(x => x.Groups).Returns(companiesMock.Object);
            IGroupManager manager = new GroupManager(kudoDbContextMock.Object);

            Group company = new Group() { Name = "company name" };

            Assert.Throws<CompanyAlreadyExistException>(() =>
            {
                manager.Add(company);
            });
        }
    }
}
