using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;

namespace iKudo.Domain.Tests
{
    public class CompanyTestsBase
    {
        protected static Mock<DbSet<Group>> ConfigureCompaniesMock(IQueryable<Group> data)
        {
            var companiesMock = new Mock<DbSet<Group>>();
            companiesMock.As<IQueryable<Group>>().Setup(x => x.Provider).Returns(data.Provider);
            companiesMock.As<IQueryable<Group>>().Setup(x => x.Expression).Returns(data.Expression);
            companiesMock.As<IQueryable<Group>>().Setup(x => x.ElementType).Returns(data.ElementType);
            companiesMock.As<IQueryable<Group>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
            return companiesMock;
        }
    }
}
