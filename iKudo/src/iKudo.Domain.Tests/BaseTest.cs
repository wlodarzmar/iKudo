using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;

namespace iKudo.Domain.Tests
{
    public class BaseTest
    {
        protected KudoDbContext DbContext { get; set; }
        protected Mock<IProvideTime> TimeProviderMock { get; private set; }

        public BaseTest()
        {
            var options = new DbContextOptionsBuilder<KudoDbContext>()
                           .UseInMemoryDatabase(Guid.NewGuid().ToString())
                           .Options;

            DbContext = new KudoDbContext(options);

            TimeProviderMock = new Mock<IProvideTime>();
        }
    }
}
