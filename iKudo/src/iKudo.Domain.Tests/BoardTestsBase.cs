using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace iKudo.Domain.Tests
{
    public class BoardTestsBase : IDisposable
    {
        protected KudoDbContext DbContext { get; set; }

        public BoardTestsBase()
        {
            var options = new DbContextOptionsBuilder<KudoDbContext>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString())
                            .Options;

            DbContext = new KudoDbContext(options);
        }

        protected static KudoDbContext CreateKudoDbContext(ICollection<Board> data = null)
        {
            var options = new DbContextOptionsBuilder<KudoDbContext>()
                            .UseInMemoryDatabase()
                            .Options;
            KudoDbContext ctx = new KudoDbContext(options);

            if (data != null)
            {
                ctx.Boards.AddRange(data);
                ctx.SaveChanges();
            }

            return ctx;
        }

        protected void FillContext(ICollection<Board> data)
        {
            DbContext.AddRange(data);
            DbContext.SaveChanges();
        }

        public void Dispose()
        {
            DbContext?.Database?.EnsureDeleted();
            DbContext?.Dispose();
        }
    }
}
