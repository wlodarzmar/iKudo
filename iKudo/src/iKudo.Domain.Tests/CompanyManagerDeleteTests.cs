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
    public class CompanyManagerDeleteTests : CompanyTestsBase
    {
        [Fact]
        public void CompanyManager_Delete_Calls_Companies_Remove_Once()
        {
            int companyId = 1;
            var data = new List<Company> { new Company { Id = companyId, Name = "company name" } }.AsQueryable();
            var companiesMock = ConfigureCompaniesMock(data);
            var dbContextMock = new Mock<KudoDbContext>();
            dbContextMock.Setup(x => x.Companies).Returns(companiesMock.Object);
            ICompanyManager manager = new CompanyManager(dbContextMock.Object);

            manager.Delete(It.IsAny<string>(), companyId);

            companiesMock.Verify(x => x.Remove(It.IsAny<Company>()), Times.Once);
            dbContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Company_Delete_Throws_NotFoundException_If_NotExist()
        {
            var data = new List<Company> { new Company { Id = 1, Name = "company name" } }.AsQueryable();
            Mock<DbSet<Company>> companiesMock = ConfigureCompaniesMock(data);
            var dbContextMock = new Mock<KudoDbContext>();
            dbContextMock.Setup(x => x.Companies).Returns(companiesMock.Object);
            ICompanyManager manager = new CompanyManager(dbContextMock.Object);

            Assert.Throws<NotFoundException>(() => manager.Delete(It.IsAny<string>(), 2));
        }

        [Fact]
        public void Company_Delete_Throws_UnauthorizedAccessException_If_Try_Delete_Foreign_Group()
        {
            var data = new List<Company> { new Company {CreatorId = "12345", Id = 1, Name = "company name" } }.AsQueryable();
            Mock<DbSet<Company>> companiesMock = ConfigureCompaniesMock(data);
            var dbContextMock = new Mock<KudoDbContext>();
            dbContextMock.Setup(x => x.Companies).Returns(companiesMock.Object);
            ICompanyManager manager = new CompanyManager(dbContextMock.Object);

            Assert.Throws<UnauthorizedAccessException>(() => manager.Delete("fakeUserId", 1));
        }
    }
}
