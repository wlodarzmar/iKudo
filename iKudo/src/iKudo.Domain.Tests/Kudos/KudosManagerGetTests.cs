using FluentAssertions;
using iKudo.Domain.Criteria;
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

            IEnumerable<Kudo> result = manager.GetKudos(new KudosSearchCriteria { BoardId = 2 });

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

            IEnumerable<Kudo> result = manager.GetKudos(new KudosSearchCriteria { UserPerformingActionId = "sender" });

            result.Where(x => string.IsNullOrWhiteSpace(x.SenderId)).Count().Should().Be(1);
        }

        [Fact]
        public void GetKudos_WithSender_ReturnsKudosOnlyWithGivenSender()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                new Kudo { BoardId = 1, SenderId = "sender1" },
                new Kudo { BoardId = 2, SenderId = "sender2" },
                new Kudo { BoardId = 3, SenderId = "sender"},
                new Kudo { BoardId = 4, SenderId = "sender" },
            };
            DbContext.Fill(existingKudos);

            IManageKudos manager = new KudosManager(DbContext, TimeProviderMock.Object);
            KudosSearchCriteria criteria = new KudosSearchCriteria
            {
                User = "sender",
                UserSearchType = UserSearchTypes.SenderOnly
            };

            IEnumerable<Kudo> result = manager.GetKudos(criteria);

            result.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_WithReceiver_ReturnsKudosOnlyWithGivenReceiver()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                new Kudo { BoardId = 1, ReceiverId = "receiver" },
                new Kudo { BoardId = 2, ReceiverId = "receiver2" },
                new Kudo { BoardId = 3, ReceiverId = "receiver" },
                new Kudo { BoardId = 4, ReceiverId = "receiver3" },
            };
            DbContext.Fill(existingKudos);

            IManageKudos manager = new KudosManager(DbContext, TimeProviderMock.Object);
            KudosSearchCriteria criteria = new KudosSearchCriteria { User = "receiver", UserSearchType = UserSearchTypes.ReceiverOnly };

            IEnumerable<Kudo> result = manager.GetKudos(criteria);

            result.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_WithUser_ReturnsKudosOnlyWithGivenReceiverOrSender()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                new Kudo { BoardId = 1, ReceiverId = "someUser" , SenderId = "sender2"},
                new Kudo { BoardId = 2, ReceiverId = "receiver2", SenderId = "sender3" },
                new Kudo { BoardId = 3, ReceiverId = "receiver3" , SenderId = "someUser"},
                new Kudo { BoardId = 4, ReceiverId = "receiver4", SenderId = "sender4" },
            };
            DbContext.Fill(existingKudos);

            IManageKudos manager = new KudosManager(DbContext, TimeProviderMock.Object);

            IEnumerable<Kudo> result = manager.GetKudos(new KudosSearchCriteria { User = "someUser" });

            result.Count().Should().Be(2);
        }
    }
}
