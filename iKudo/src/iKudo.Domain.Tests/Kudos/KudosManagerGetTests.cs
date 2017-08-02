using FluentAssertions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Kudos
{
    public class KudosManagerGetTests : BaseTest
    {
        [Fact]
        public void Get_WithGivenBoardId_ReturnsValidKudos()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                new Kudo { BoardId = 1 },
                new Kudo { BoardId = 2 },
                new Kudo { BoardId = 2 },
                new Kudo { BoardId = 3 },
            };
            DbContext.Fill(existingKudos);

            IManageKudos manager = new KudosManager(DbContext, TimeProviderMock.Object);

            IEnumerable<Kudo> result = manager.GetKudos(2);

            result.Count().Should().Be(2);
        }
    }
}
