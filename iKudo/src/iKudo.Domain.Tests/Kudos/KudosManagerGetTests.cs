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

            IEnumerable<Kudo> result = manager.GetKudos(null, 2);

            result.Count().Should().Be(2);
        }

        [Fact]
        public void Get_WithUserIdPerformingAction_ReturnsSenderInAnonymousKudosOnlyIfUserIsSender()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                new Kudo { BoardId = 1, SenderId = "sender1", IsAnonymous = false },
                new Kudo { BoardId = 2, SenderId = "sender2", IsAnonymous = true },
                new Kudo { BoardId = 4, SenderId = "sender", IsAnonymous = true },
                new Kudo { BoardId = 4, SenderId = "sender", IsAnonymous = false },
            };
            DbContext.Fill(existingKudos);

            IManageKudos manager = new KudosManager(DbContext, TimeProviderMock.Object);

            IEnumerable<Kudo> result = manager.GetKudos("sender");

            result.Where(x => string.IsNullOrWhiteSpace(x.SenderId)).Count().Should().Be(1);
        }
    }
}
