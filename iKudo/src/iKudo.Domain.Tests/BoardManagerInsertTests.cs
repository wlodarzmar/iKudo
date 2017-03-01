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
    public class BoardManagerInsertTests : BoardTestsBase
    {
        [Fact]
        public void CompanyManager_Throws_ArgumentNullException_If_Company_Is_Null()
        {
            var kudoDbContextMock = new Mock<KudoDbContext>();
            IBoardManager manager = new BoardManager(kudoDbContextMock.Object);

            Assert.Throws(typeof(ArgumentNullException), () => manager.Add(null));
        }

        [Fact]
        public void CompanyManager_InsertCompany_Calls_Companies_Add_Once()
        {
            var data = new List<Board> { new Board { Name = "name" } }.AsQueryable();
            var boardsMock = ConfigureBoardsMock(data);
            var kudoDbContextMock = new Mock<KudoDbContext>();
            kudoDbContextMock.Setup(x => x.Boards).Returns(boardsMock.Object);
            IBoardManager manager = new BoardManager(kudoDbContextMock.Object);

            Board company = new Board() { Name = "company name" };
            manager.Add(company);

            boardsMock.Verify(x => x.Add(It.Is<Board>(c => c.Name == company.Name)), Times.Once);
            kudoDbContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CompanyManager_InsertCompany_Returns_AddedCompany()
        {
            DateTime now = DateTime.Now;
            var data = new List<Board> { new Board { Name = "name", } }.AsQueryable();
            var companiesMock = ConfigureBoardsMock(data);
            var kudoDbContextMock = new Mock<KudoDbContext>();
            kudoDbContextMock.Setup(x => x.Boards).Returns(companiesMock.Object);
            IBoardManager manager = new BoardManager(kudoDbContextMock.Object);

            Board company = new Board() { Name = "company name" };
            Board addedCompany = manager.Add(company);

            Assert.NotNull(addedCompany);
            Assert.Equal(company.Name, addedCompany.Name);
            Assert.True(company.CreationDate >= now);
        }

        [Fact]
        public void CompanyManager_InsertCompany_Throws_Exception_If_Company_Name_Exists()
        {
            var data = new List<Board> { new Board { Name = "company name" } }.AsQueryable();
            var companiesMock = ConfigureBoardsMock(data);
            var kudoDbContextMock = new Mock<KudoDbContext>();
            kudoDbContextMock.Setup(x => x.Boards).Returns(companiesMock.Object);
            IBoardManager manager = new BoardManager(kudoDbContextMock.Object);

            Board company = new Board() { Name = "company name" };

            Assert.Throws<AlreadyExistException>(() =>
            {
                manager.Add(company);
            });
        }
    }
}
