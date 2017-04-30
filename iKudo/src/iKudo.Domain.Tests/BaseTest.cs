using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iKudo.Domain.Tests
{
    public class BaseTest : IDisposable
    {
        protected KudoDbContext DbContext { get; set; }

        public BaseTest()
        {
            var options = new DbContextOptionsBuilder<KudoDbContext>()
                           .UseInMemoryDatabase(Guid.NewGuid().ToString())
                           .Options;

            DbContext = new KudoDbContext(options);
        }

        public void Dispose()
        {
            DbContext?.Database?.EnsureDeleted();
            DbContext?.Dispose();
        }
    }
}
