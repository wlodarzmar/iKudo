using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iKudo.Domain.Tests
{
    public class BoardTestsBase : BaseTest
    {
        public BoardTestsBase()
        {
            TimeProviderMock = new Mock<ITimeProvider>();
        }

        protected Mock<ITimeProvider> TimeProviderMock { get; private set; }

        protected void FillContext(ICollection<Board> data)
        {
            DbContext.AddRange(data);
            DbContext.SaveChanges();
        }
        
        protected void FillContext(ICollection<JoinRequest> data)
        {
            DbContext.AddRange(data);
            DbContext.SaveChanges();
        }
    }
}
