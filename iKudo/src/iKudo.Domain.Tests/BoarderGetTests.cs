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
    public class BoardManagerGetTests : BoardTestsBase
    {
        IQueryable<Board> data = new List<Board> {
            new Board { Id = 1, Name = "company name" },
            new Board { Id = 2, Name = "company name 2" }
        }.AsQueryable();

        Mock<DbSet<Board>> companiesMock = new Mock<DbSet<Board>>();
        Mock<KudoDbContext> kudoDbContextMock = new Mock<KudoDbContext>();

        public BoardManagerGetTests()
        {
            companiesMock = ConfigureBoardsMock(data);
            kudoDbContextMock.Setup(x => x.Boards).Returns(companiesMock.Object);
        }

        [Fact]
        public void CompanyManager_GetCompany_Returns_Null_If_Not_Found_Company()
        {
            IBoardManager manager = new BoardManager(kudoDbContextMock.Object);

            int companyId = 3;
            Board company = manager.Get(companyId);

            Assert.Null(company);
        }

        [Fact]
        public void CompanyManager_GetCompany_Returns_Company()
        {
            IBoardManager manager = new BoardManager(kudoDbContextMock.Object);

            int companyId = 1;
            Board company = manager.Get(companyId);

            Assert.NotNull(company);
        }

        [Fact]
        public void CompanyManager_GetAll_ReturnsAllCompanies()
        {
            IBoardManager manager = new BoardManager(kudoDbContextMock.Object);

            ICollection<Board> companies = manager.GetAll();

            Assert.NotNull(companies);
            Assert.Equal(data.Count(), companies.Count);
        }
    }
}
