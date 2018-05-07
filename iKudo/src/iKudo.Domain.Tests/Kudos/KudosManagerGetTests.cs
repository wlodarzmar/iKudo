using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using Moq;
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
                CreateKudo(1),
                CreateKudo(2),
                CreateKudo(2),
                CreateKudo(3),
            };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> result = Manager.GetKudos(new KudosSearchCriteria { BoardId = 2 });

            result.Count().Should().Be(2);
        }

        [Fact]
        public void Get_WithUserIdPerformingAction_ReturnsSenderInAnonymousKudosOnlyIfUserIsSender()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                CreateKudo(1, boardCreator: "user", senderId: "sender1", isAnonymous: false),
                CreateKudo(2, boardCreator: "user", senderId: "sender2", isAnonymous: true),
                CreateKudo(3, boardCreator: "user", senderId: "user", isAnonymous: true),
                CreateKudo(4, boardCreator: "user", senderId: "sender", isAnonymous: false),
            };
            DbContext.Fill(existingKudos);


            IEnumerable<Kudo> result = Manager.GetKudos(new KudosSearchCriteria { UserPerformingActionId = "user" });

            result.Count().Should().Be(4);
            result.Count(x => string.IsNullOrWhiteSpace(x.SenderId)).Should().Be(1);
            result.Count(x => x.Sender == null).Should().Be(1);
        }

        [Fact]
        public void GetKudos_WithSender_ReturnsKudosOnlyWithGivenSender()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                CreateKudo(1, "", "sender1", false),
                CreateKudo(2, "", "sender2", false),
                CreateKudo(3, "", "sender", false),
                CreateKudo(4, "", "sender", false),
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
                CreateKudo(1,"", "", "receiver", false),
                CreateKudo(2,"", "", "receiver2", false),
                CreateKudo(3,"", "", "receiver", false),
                CreateKudo(4,"", "", "receiver3", false),
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
                CreateKudo(1, string.Empty, "sender2", "someUser", false),
                CreateKudo(2, string.Empty, "sender3", "receiver2", false),
                CreateKudo(3, string.Empty, "someUser", "receiver3", false),
                CreateKudo(4, string.Empty, "sender4", "receiver4", false),
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
                CreateKudo(board1, "sender2", "someUser" , false),
                CreateKudo(board2, "sender3", "receiver2", false),
                CreateKudo(board3, "sender3", "receiver3", false),
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
                CreateKudo(board1, "sender2", "someUser",false),
                CreateKudo(board2, "sender3", "receiver2",false),
            };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> result = Manager.GetKudos(new KudosSearchCriteria());

            result.Count().Should().Be(1);
        }

        [Fact]
        public void GetKudos_WithStatus_ReturnsKudosWithGivenStatus()
        {
            Board board1 = new Board { Id = 1, CreatorId = "user" };
            var kudo1 = CreateKudo(board1, "sender2", "someUser", false);
            kudo1.Status = KudoStatus.New;
            var kudo2 = CreateKudo(board1, "sender3", "receiver2", false);
            kudo2.Status = KudoStatus.Accepted;
            List<Kudo> existingKudos = new List<Kudo> { kudo1, kudo2 };
            DbContext.Fill(existingKudos);

            var criteria = new KudosSearchCriteria { Status = KudoStatus.Accepted };
            IEnumerable<Kudo> result = Manager.GetKudos(criteria);

            result.Count().Should().Be(1);
        }

        [Fact]
        public void GetKudos_ReturnsKudosWithNewStatus_WhenUserIsSenderOrReceiver()
        {
            Board board1 = new Board { Id = 1, CreatorId = "user" };
            var kudo1 = CreateKudo(board1, "user2", "someUser", false);
            kudo1.Status = KudoStatus.New;
            var kudo2 = CreateKudo(board1, "sender3", "user2", false);
            kudo2.Status = KudoStatus.New;
            var kudo3 = CreateKudo(board1, "otherUser", "otherUser2", false);
            kudo3.Status = KudoStatus.New;
            List<Kudo> existingKudos = new List<Kudo> { kudo1, kudo2, kudo3 };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> kudos = Manager.GetKudos(new KudosSearchCriteria { UserPerformingActionId = "user2" });

            kudos.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_DecryptsKudoContent()
        {
            Board board1 = new Board { Id = 1 };
            List<Kudo> existingKudos = new List<Kudo>
            {
                CreateKudo(board1, "sender", "receiver", false, "desc"),
                CreateKudo(board1, "sender", "receiver", false, "desc"),
                CreateKudo(board1, "sender", "receiver", false, "desc"),
            };
            DbContext.Fill(existingKudos);
            var decryptedKudo = CreateKudo(board1, "sender", "receiver", false, "decryptedDesc");

            IEnumerable<Kudo> kudos = Manager.GetKudos(new KudosSearchCriteria { BoardId = 1 });

            KudoCypherMock.Verify(x => x.Decrypt(It.IsAny<Kudo>()), Times.Exactly(kudos.Count()));
        }
    }
}
