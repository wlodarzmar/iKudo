using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iKudo.Domain.Tests
{
    public class BaseTest : IDisposable
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

        public void Dispose()
        {
            DbContext?.Database?.EnsureDeleted();
            DbContext?.Dispose();
        }

    }
}
