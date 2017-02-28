using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;

namespace iKudo.Domain.Tests
{
    public class BoardTestsBase
    {
        protected static Mock<DbSet<Board>> ConfigureCompaniesMock(IQueryable<Board> data)
        {
            var companiesMock = new Mock<DbSet<Board>>();
            companiesMock.As<IQueryable<Board>>().Setup(x => x.Provider).Returns(data.Provider);
            companiesMock.As<IQueryable<Board>>().Setup(x => x.Expression).Returns(data.Expression);
            companiesMock.As<IQueryable<Board>>().Setup(x => x.ElementType).Returns(data.ElementType);
            companiesMock.As<IQueryable<Board>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
            return companiesMock;
        }
    }
}
