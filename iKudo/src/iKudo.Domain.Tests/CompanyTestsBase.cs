using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;

namespace iKudo.Domain.Tests
{
    public class CompanyTestsBase
    {
        protected static Mock<DbSet<Company>> ConfigureCompaniesMock(IQueryable<Company> data)
        {
            var companiesMock = new Mock<DbSet<Company>>();
            companiesMock.As<IQueryable<Company>>().Setup(x => x.Provider).Returns(data.Provider);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.Expression).Returns(data.Expression);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.ElementType).Returns(data.ElementType);
            companiesMock.As<IQueryable<Company>>().Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
            return companiesMock;
        }
    }
}
