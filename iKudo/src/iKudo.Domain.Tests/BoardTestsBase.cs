using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;

namespace iKudo.Domain.Tests
{
    public class BoardTestsBase
    {
        protected static Mock<DbSet<Board>> ConfigureBoardsMock(IQueryable<Board> data)
        {
            var boardsMock = new Mock<DbSet<Board>>(MockBehavior.Strict);
            boardsMock.As<IQueryable<Board>>().Setup(x => x.Provider).Returns(data.Provider);
            boardsMock.As<IQueryable<Board>>().Setup(x => x.Expression).Returns(data.Expression);
            boardsMock.As<IQueryable<Board>>().Setup(x => x.ElementType).Returns(data.ElementType);
            boardsMock.As<IQueryable<Board>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
            return boardsMock;
        }
    }
}
