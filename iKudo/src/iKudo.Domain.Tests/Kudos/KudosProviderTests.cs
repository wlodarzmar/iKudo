using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using iKudo.Domain.Tests.Extensions;
using iKudo.Domain.Tests.Helpers;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Kudos
{
    public class KudosProviderTests : BaseTest
    {
        protected IProvideKudos Provider { get; private set; }
        protected Mock<IFileStorage> FileStorageMock { get; private set; }
        protected Mock<IKudoCypher> KudoCypherMock { get; private set; }

        public KudosProviderTests()
        {
            FileStorageMock = new Mock<IFileStorage>();
            KudoCypherMock = new Mock<IKudoCypher>();
            Provider = new KudosProvider(DbContext, KudoCypherMock.Object, FileStorageMock.Object);
        }

        [Fact]
        public void Get_WithGivenBoardId_ReturnsValidKudos()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                KudosHelper.CreateKudo(1),
                KudosHelper.CreateKudo(2),
                KudosHelper.CreateKudo(2),
                KudosHelper.CreateKudo(3),
            };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> result = Provider.GetKudos(new KudosSearchCriteria { BoardId = 2 });

            result.Count().Should().Be(2);
        }

        [Fact]
        public void Get_WithUserIdPerformingAction_ReturnsSenderInAnonymousKudosOnlyIfUserIsSender()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                KudosHelper.CreateKudo(1, boardCreator: "user", senderId: "sender1", isAnonymous: false),
                KudosHelper.CreateKudo(2, boardCreator: "user", senderId: "sender2", isAnonymous: true),
                KudosHelper.CreateKudo(3, boardCreator: "user", senderId: "user", isAnonymous: true),
                KudosHelper.CreateKudo(4, boardCreator: "user", senderId: "sender", isAnonymous: false),
            };
            DbContext.Fill(existingKudos);


            IEnumerable<Kudo> result = Provider.GetKudos(new KudosSearchCriteria { UserPerformingActionId = "user" });

            result.Count().Should().Be(4);
            result.Count(x => string.IsNullOrWhiteSpace(x.SenderId)).Should().Be(1);
            result.Count(x => x.Sender == null).Should().Be(1);
        }

        [Fact]
        public void GetKudos_WithSender_ReturnsKudosOnlyWithGivenSender()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                KudosHelper.CreateKudo(1, "", "sender1", false),
                KudosHelper.CreateKudo(2, "", "sender2", false),
                KudosHelper.CreateKudo(3, "", "sender", false),
                KudosHelper.CreateKudo(4, "", "sender", false),
            };
            DbContext.Fill(existingKudos);

            KudosSearchCriteria criteria = new KudosSearchCriteria
            {
                User = "sender",
                UserSearchType = UserSearchTypes.SenderOnly
            };

            IEnumerable<Kudo> result = Provider.GetKudos(criteria);

            result.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_WithReceiver_ReturnsKudosOnlyWithGivenReceiver()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                KudosHelper.CreateKudo(1,"", "", "receiver", false),
                KudosHelper.CreateKudo(2,"", "", "receiver2", false),
                KudosHelper.CreateKudo(3,"", "", "receiver", false),
                KudosHelper.CreateKudo(4,"", "", "receiver3", false),
            };
            DbContext.Fill(existingKudos);

            KudosSearchCriteria criteria = new KudosSearchCriteria { User = "receiver", UserSearchType = UserSearchTypes.ReceiverOnly };

            IEnumerable<Kudo> result = Provider.GetKudos(criteria);

            result.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_WithUser_ReturnsKudosOnlyWithGivenReceiverOrSender()
        {
            List<Kudo> existingKudos = new List<Kudo> {
                KudosHelper.CreateKudo(1, string.Empty, "sender2", "someUser", false),
                KudosHelper.CreateKudo(2, string.Empty, "sender3", "receiver2", false),
                KudosHelper.CreateKudo(3, string.Empty, "someUser", "receiver3", false),
                KudosHelper.CreateKudo(4, string.Empty, "sender4", "receiver4", false),
            };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> result = Provider.GetKudos(new KudosSearchCriteria { User = "someUser" });

            result.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_WithUserPerformingActionIdAndPrivateBoard_ReturnsKudosFromPublicOrOwnedBoards()
        {
            Board board1 = new Board { Id = 1, IsPrivate = true, CreatorId = "user" };
            Board board2 = new Board { Id = 2, IsPrivate = false, CreatorId = "user2" };
            Board board3 = new Board { Id = 3, IsPrivate = true, CreatorId = "user3" };
            List<Kudo> existingKudos = new List<Kudo> {
                KudosHelper.CreateKudo(board1, "sender2", "someUser" , false),
                KudosHelper.CreateKudo(board2, "sender3", "receiver2", false),
                KudosHelper.CreateKudo(board3, "sender3", "receiver3", false),
            };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> result = Provider.GetKudos(new KudosSearchCriteria { UserPerformingActionId = "user" });

            result.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_WithoutUserPerformingAction_ReturnsOnlyKudosFromPublicBoards()
        {
            Board board1 = new Board { Id = 1, IsPrivate = true, CreatorId = "user" };
            Board board2 = new Board { Id = 2, IsPrivate = false, CreatorId = "user2" };
            List<Kudo> existingKudos = new List<Kudo> {
                KudosHelper.CreateKudo(board1, "sender2", "someUser",false),
                KudosHelper.CreateKudo(board2, "sender3", "receiver2",false),
            };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> result = Provider.GetKudos(new KudosSearchCriteria());

            result.Count().Should().Be(1);
        }

        [Fact]
        public void GetKudos_WithStatus_ReturnsKudosWithGivenStatus()
        {
            Board board1 = new Board { Id = 1, CreatorId = "user" };
            var kudo1 = KudosHelper.CreateKudo(board1, "sender2", "someUser", false);
            kudo1.Status = KudoStatus.New;
            var kudo2 = KudosHelper.CreateKudo(board1, "sender3", "receiver2", false);
            kudo2.Status = KudoStatus.Accepted;
            List<Kudo> existingKudos = new List<Kudo> { kudo1, kudo2 };
            DbContext.Fill(existingKudos);

            var criteria = new KudosSearchCriteria { Statuses = new[] { KudoStatus.Accepted } };
            IEnumerable<Kudo> result = Provider.GetKudos(criteria);

            result.Count().Should().Be(1);
        }

        [Fact]
        public void GetKudos_ReturnsKudosWithNewStatus_WhenUserIsSenderOrReceiver()
        {
            Board board1 = new Board { Id = 1, CreatorId = "user" };
            var kudo1 = KudosHelper.CreateKudo(board1, "user2", "someUser", false);
            kudo1.Status = KudoStatus.New;
            var kudo2 = KudosHelper.CreateKudo(board1, "sender3", "user2", false);
            kudo2.Status = KudoStatus.New;
            var kudo3 = KudosHelper.CreateKudo(board1, "otherUser", "otherUser2", false);
            kudo3.Status = KudoStatus.New;
            List<Kudo> existingKudos = new List<Kudo> { kudo1, kudo2, kudo3 };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> kudos = Provider.GetKudos(new KudosSearchCriteria { UserPerformingActionId = "user2" });

            kudos.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_ReturnsKudosWithNewStatus_WhenUserBoardOwner()
        {
            Board board1 = new Board { Id = 1, CreatorId = "creator" };
            var kudo1 = KudosHelper.CreateKudo(board1, "sender", "receiver", false);
            kudo1.Status = KudoStatus.New;
            var kudo2 = KudosHelper.CreateKudo(board1, "sender2", "receiver2", false);
            kudo2.Status = KudoStatus.New;
            DbContext.Fill(kudo1, kudo2);

            IEnumerable<Kudo> kudos = Provider.GetKudos(new KudosSearchCriteria { UserPerformingActionId = "creator" });

            kudos.Count().Should().Be(2);
        }

        [Fact]
        public void GetKudos_DecryptsKudoContent()
        {
            Board board1 = new Board { Id = 1 };
            List<Kudo> existingKudos = new List<Kudo>
            {
                KudosHelper.CreateKudo(board1, "sender", "receiver", false, "desc"),
                KudosHelper.CreateKudo(board1, "sender", "receiver", false, "desc"),
                KudosHelper.CreateKudo(board1, "sender", "receiver", false, "desc"),
            };
            DbContext.Fill(existingKudos);
            KudosHelper.CreateKudo(board1, "sender", "receiver", false, "decryptedDesc");

            IEnumerable<Kudo> kudos = Provider.GetKudos(new KudosSearchCriteria { BoardId = 1 });

            KudoCypherMock.Verify(x => x.Decrypt(It.IsAny<Kudo>()), Times.Exactly(kudos.Count()));
        }

        [Fact]
        public void GetKudos_WithNewStatus_ReturnsKudosWithNewOnlyIfItIsCreatorOfKudo()
        {
            Board board = new Board { Id = 1 };
            List<Kudo> existingKudos = new List<Kudo> {
                KudosHelper.CreateKudo(board, "user", "receiver", false).WithStatus(KudoStatus.New),
                KudosHelper.CreateKudo(board, "otherUser", "receiver", false).WithStatus(KudoStatus.New),
                KudosHelper.CreateKudo(board, "user", "receiver", false).WithStatus(KudoStatus.Rejected),
                KudosHelper.CreateKudo(board, "user", "receiver", false).WithStatus(KudoStatus.Accepted),
            };
            DbContext.Fill(existingKudos);

            IEnumerable<Kudo> kudos = Provider.GetKudos(new KudosSearchCriteria
            {
                UserPerformingActionId = "user",
                Statuses = new[] { KudoStatus.New },
                BoardId = 1,
            });

            kudos.Count().Should().Be(1);
        }

        [Fact]
        public void GetKudos_PrivateBoardGetByBoardId_ReturnKudosForBoardMember()
        {
            Board board = new Board
            {
                Id = 1,
                CreatorId = "creator",
                IsPrivate = true,
                UserBoards = new List<UserBoard> { new UserBoard("sender", 1), new UserBoard("creator", 1) }
            };
            Kudo kudo = KudosHelper.CreateKudo(board, "sender", "receiver", false);
            DbContext.Fill(kudo);

            IEnumerable<Kudo> kudos = Provider.GetKudos(new KudosSearchCriteria { BoardId = 1, UserPerformingActionId = "sender" });

            kudos.Count().Should().Be(1);
        }

        [Fact]
        public void GetKudos_PrivateBoardGetByBoardIdKudosNew_DoesntReturnThisKudo()
        {
            Board board = new Board
            {
                Id = 1,
                CreatorId = "creator",
                IsPrivate = true,
                UserBoards = new List<UserBoard> { new UserBoard("sender", 1), new UserBoard("creator", 1) }
            };
            Kudo kudo = KudosHelper.CreateKudo(board, "sender", "receiver", false);
            kudo.Status = KudoStatus.New;
            DbContext.Fill(kudo);

            IEnumerable<Kudo> kudos = Provider.GetKudos(new KudosSearchCriteria { BoardId = 1, UserPerformingActionId = "sender" });

            kudos.Count().Should().Be(0);
        }
    }
}
