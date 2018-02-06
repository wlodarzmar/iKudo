using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Kudos
{
    public class KudosManagerGetTests : KudosManagerBaseTest
    {
        [Fact]
        public void Get_WithGivenBoardId_ReturnsValidKudos()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                new Kudo { BoardId = 1, Board = new Board{Id = 1 } },
                new Kudo { BoardId = 2, Board = new Board{Id = 2 } },
                new Kudo { BoardId = 2, Board = new Board{Id = 2 } },
                new Kudo { BoardId = 3, Board = new Board{Id = 3 } },
            };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> result = Manager.GetKudos(new KudosSearchCriteria { BoardId = 2 });

            result.Count().Should().Be(2);
        }

        [Fact]
        public void Get_WithUserIdPerformingAction_ReturnsSenderInAnonymousKudosOnlyIfUserIsSender()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                new Kudo { BoardId = 1, Board = new Board{Id = 1, CreatorId = "sender" }, SenderId = "sender1", IsAnonymous = false },
                new Kudo { BoardId = 2, Board = new Board{Id = 2, CreatorId = "sender" }, SenderId = "sender2", IsAnonymous = true },
                new Kudo { BoardId = 4, Board = new Board{Id = 4, CreatorId = "sender" }, SenderId = "sender", IsAnonymous = true },
                new Kudo { BoardId = 4, Board = new Board{Id = 5, CreatorId = "sender" }, SenderId = "sender", IsAnonymous = false },
            };
            DbContext.Fill(existingKudos);


            IEnumerable<Kudo> result = Manager.GetKudos(new KudosSearchCriteria { UserPerformingActionId = "sender" });

            result.Count().Should().Be(4);
            result.Where(x => string.IsNullOrWhiteSpace(x.SenderId)).Count().Should().Be(1);
        }

        [Fact]
        public void GetKudos_WithSender_ReturnsKudosOnlyWithGivenSender()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                new Kudo { BoardId = 1,Board = new Board{Id = 1},SenderId = "sender1" },
                new Kudo { BoardId = 2,Board = new Board{Id = 2},SenderId = "sender2" },
                new Kudo { BoardId = 3,Board = new Board{Id = 3},SenderId = "sender"},
                new Kudo { BoardId = 4,Board = new Board{Id = 4}, SenderId = "sender" },
            };
            DbContext.Fill(existingKudos);

            KudosSearchCriteria criteria = new KudosSearchCriteria
            {
                User = "sender",
                UserSearchType = UserSearchTypes.SenderOnly
            };

            IEnumerable<Kudo> result = Manager.GetKudos(criteria);

            result.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_WithReceiver_ReturnsKudosOnlyWithGivenReceiver()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                new Kudo { BoardId = 1,Board = new Board{Id = 1}, ReceiverId = "receiver" },
                new Kudo { BoardId = 2,Board = new Board{Id = 2}, ReceiverId = "receiver2" },
                new Kudo { BoardId = 3,Board = new Board{Id = 3}, ReceiverId = "receiver" },
                new Kudo { BoardId = 4,Board = new Board{Id = 4}, ReceiverId = "receiver3" },
            };
            DbContext.Fill(existingKudos);

            KudosSearchCriteria criteria = new KudosSearchCriteria { User = "receiver", UserSearchType = UserSearchTypes.ReceiverOnly };

            IEnumerable<Kudo> result = Manager.GetKudos(criteria);

            result.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_WithUser_ReturnsKudosOnlyWithGivenReceiverOrSender()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                new Kudo { BoardId = 1, Board = new Board{Id = 1}, ReceiverId = "someUser" , SenderId = "sender2"},
                new Kudo { BoardId = 2, Board = new Board{Id = 2}, ReceiverId = "receiver2", SenderId = "sender3" },
                new Kudo { BoardId = 3, Board = new Board{Id = 3}, ReceiverId = "receiver3" , SenderId = "someUser"},
                new Kudo { BoardId = 4, Board = new Board{Id = 4}, ReceiverId = "receiver4", SenderId = "sender4" },
            };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> result = Manager.GetKudos(new KudosSearchCriteria { User = "someUser" });

            result.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_WithUserPerformingActionIdAndPrivateBoard_ReturnsKudosFromPublicOrOwnedBoards()
        {
            Board board1 = new Board { Id = 1, IsPrivate = true, CreatorId = "user" };
            Board board2 = new Board { Id = 2, IsPrivate = false, CreatorId = "user2" };
            Board board3 = new Board { Id = 3, IsPrivate = true, CreatorId = "user3" };
            List<Kudo> existingKudos = new List<Kudo> {
                new Kudo { BoardId = 1, Board = board1, ReceiverId = "someUser" , SenderId = "sender2"},
                new Kudo { BoardId = 2, Board = board2, ReceiverId = "receiver2", SenderId = "sender3" },
                new Kudo { BoardId = 3, Board = board3, ReceiverId = "receiver2", SenderId = "sender3" },
            };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> result = Manager.GetKudos(new KudosSearchCriteria { UserPerformingActionId = "user" });

            result.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_WithoutUserPerformingAction_ReturnsOnlyKudosFromPublicBoards()
        {
            Board board1 = new Board { Id = 1, IsPrivate = true, CreatorId = "user" };
            Board board2 = new Board { Id = 2, IsPrivate = false, CreatorId = "user2" };
            List<Kudo> existingKudos = new List<Kudo> {
                new Kudo { BoardId = 1, Board = board1, ReceiverId = "someUser" , SenderId = "sender2"},
                new Kudo { BoardId = 2, Board = board2, ReceiverId = "receiver2", SenderId = "sender3" },
            };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> result = Manager.GetKudos(new KudosSearchCriteria());

            result.Count().Should().Be(1);
        }
    }
}
