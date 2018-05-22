using FluentAssertions;
using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Kudos
{
    public class KudosManagerAddKudoTests : KudosManagerBaseTest
    {
        private readonly Board existingBoardPrivate = new Board
        {
            Id = 1,
            Name = "board",
            UserBoards = new List<UserBoard> { new UserBoard("sender", 1), new UserBoard("receiver", 1) },
            IsPrivate = true
        };
        private readonly Board existingBoard2Private = new Board
        {
            Id = 2,
            Name = "board2",
            UserBoards = new List<UserBoard> { new UserBoard("sender2", 2), new UserBoard("receiver", 2) },
            IsPrivate = true
        };
        private readonly Board existingBoard3Public = new Board
        {
            Id = 3,
            Name = "board2",
            UserBoards = new List<UserBoard> { new UserBoard("receiver", 3) },
            IsPrivate = false
        };

        public KudosManagerAddKudoTests()
        {
            DbContext.Fill(new List<Board> { existingBoardPrivate, existingBoard2Private, existingBoard3Public });
        }

        [Fact]
        public void AddKudo_ValidKudo_KudoAdded()
        {
            Kudo kudo = new Kudo
            {
                Board = existingBoardPrivate,
                BoardId = existingBoardPrivate.Id,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = true,
                ReceiverId = "receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob,
            };

            kudo = Manager.Add(kudo.SenderId, kudo);

            DbContext.Kudos.FirstOrDefault(x => x.Id == kudo.Id).Should().NotBeNull();
        }

        [Fact]
        public void AddKudo_UserTriesToAddKudoToForeignPrivateBoard_ThrowsUnauthorizedAccessException()
        {
            Kudo kudo = new Kudo
            {
                BoardId = 2,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = true,
                ReceiverId = "receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob
            };

            Manager.Invoking(x => x.Add(kudo.SenderId, kudo)).ShouldThrow<UnauthorizedAccessException>();
        }

        [Fact]
        public void AddKudo_UserCanAddKudoToForeignPublicBoard_KudoAdded()
        {
            Kudo kudo = new Kudo
            {
                BoardId = 3,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = true,
                ReceiverId = "receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob
            };

            Manager.Add(kudo.SenderId, kudo);

            DbContext.Kudos.FirstOrDefault(x => x.Id == kudo.Id).Should().NotBeNull();
        }

        [Fact]
        public void AddKudo_UserTriesToAddKudoForUserThatIsNotMemberOfBoard_ThrowsInvalidOperationException()
        {
            Kudo kudo = new Kudo
            {
                BoardId = existingBoardPrivate.Id,
                Board = existingBoardPrivate,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = true,
                ReceiverId = "some other receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob
            };

            Manager.Invoking(x => x.Add(kudo.SenderId, kudo)).ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void AddKudo_ValidKudo_AddsNotificationAboutNewKudo()
        {
            Kudo kudo = new Kudo
            {
                Board = existingBoardPrivate,
                BoardId = existingBoardPrivate.Id,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = false,
                ReceiverId = "receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob,
            };

            kudo = Manager.Add(kudo.SenderId, kudo);

            DbContext.Notifications.Any(x => x.BoardId == existingBoardPrivate.Id
                                        && x.ReceiverId == kudo.ReceiverId
                                        && x.SenderId == kudo.SenderId
                                        && x.Type == NotificationTypes.KudoAdded)
                                   .Should().BeTrue();
        }

        [Fact]
        public void AddKudo_ValidKudo_AddsAnonymousNotificationAboutNewKudo()
        {
            Kudo kudo = new Kudo
            {
                Board = existingBoardPrivate,
                BoardId = existingBoardPrivate.Id,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = true,
                ReceiverId = "receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob,
            };

            kudo = Manager.Add(kudo.SenderId, kudo);

            DbContext.Notifications.Any(x => x.BoardId == existingBoardPrivate.Id
                                        && x.ReceiverId == kudo.ReceiverId
                                        && x.SenderId == kudo.SenderId
                                        && x.Type == NotificationTypes.AnonymousKudoAdded)
                                   .Should().BeTrue();
        }

        [Fact]
        public void AddKudo_UserPerformingActionDifferentThanKudoSender_ThrowsForbidden()
        {
            Kudo kudo = new Kudo
            {
                Board = existingBoardPrivate,
                BoardId = existingBoardPrivate.Id,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = true,
                ReceiverId = "receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob
            };
            Manager.Invoking(x => x.Add("otherUser", kudo)).ShouldThrow<UnauthorizedAccessException>();
        }

        [Fact]
        public void AddKudo_KudoWithoutCreationDate_SetsCreationDate()
        {
            Kudo kudo = new Kudo
            {
                Board = existingBoardPrivate,
                BoardId = existingBoardPrivate.Id,
                Description = "desc",
                ReceiverId = "receiver",
                SenderId = "sender",
                Type = KudoType.Congratulations,
            };
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);

            kudo = Manager.Add("sender", kudo);

            kudo.CreationDate.Should().Be(date);
        }

        [Fact]
        public void AddKudo_KudoWithImage_CallsSaveOnFileStorage()
        {
            Kudo kudo = new Kudo
            {
                Board = existingBoardPrivate,
                BoardId = existingBoardPrivate.Id,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = false,
                ReceiverId = "receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob,
                Image = "imagebase64"
            };
            Manager.Add(kudo.SenderId, kudo);

            FileStorageMock.Verify(x => x.Save(It.IsAny<string>(), It.IsAny<byte[]>()));
        }

        [Fact]
        public void AddKudo_EncryptsContent()
        {
            Kudo kudo = new Kudo
            {
                Board = existingBoardPrivate,
                BoardId = existingBoardPrivate.Id,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = false,
                ReceiverId = "receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob,
            };
            Manager.Add(kudo.SenderId, kudo);

            KudoCypherMock.Verify(x => x.Encrypt(It.IsAny<Kudo>()));
        }

        [Fact]
        public void AddKudo_PrivateBoardNoAcceptance_AddsKudosWithStatusAccepted()
        {
            Board board = CreateBoard(5, "boardName", "creator", new[] { "sender", "receiver" })
                            .WithPublicity(false)
                            .WithAcceptance(AcceptanceType.None);
            DbContext.Fill(new List<Board> { board });
            var kudo = CreateKudo(board, "sender", "receiver", false);

            var addedKudo = Manager.Add(kudo.SenderId, kudo);

            addedKudo.Status.Should().Be(KudoStatus.Accepted);
        }

        [Fact]
        public void AddKudo_PublicBoardNoAcceptanceExternalUser_AddsKudosWithStatusAccepted()
        {
            Board board = CreateBoard(5, "boardName", "creator", new[] { "sender", "receiver" })
                            .WithPublicity(true)
                            .WithAcceptance(AcceptanceType.None);
            DbContext.Fill(new List<Board> { board });
            var kudo = CreateKudo(board, "externalUser", "receiver", false);

            var addedKudo = Manager.Add(kudo.SenderId, kudo);

            addedKudo.Status.Should().Be(KudoStatus.Accepted);
        }

        [Fact]
        public void AddKudo_PublicBoardNoAcceptanceInternalUser_AddsKudosWithStatusAccepted()
        {
            Board board = CreateBoard(5, "boardName", "creator", new[] { "sender", "receiver" })
                            .WithPublicity(true)
                            .WithAcceptance(AcceptanceType.None);
            DbContext.Fill(new List<Board> { board });
            var kudo = CreateKudo(board, "sender", "receiver", false);

            var addedKudo = Manager.Add(kudo.SenderId, kudo);

            addedKudo.Status.Should().Be(KudoStatus.Accepted);
        }

        [Fact]
        public void AddKudo_PrivateBoardAcceptanceAll_AddsKudosWithStatusNew()
        {
            Board board = CreateBoard(5, "boardName", "creator", new[] { "sender", "receiver" })
                            .WithPublicity(false)
                            .WithAcceptance(AcceptanceType.All);
            DbContext.Fill(new List<Board> { board });
            var kudo = CreateKudo(board, "sender", "receiver", false);

            var addedKudo = Manager.Add(kudo.SenderId, kudo);

            addedKudo.Status.Should().Be(KudoStatus.New);
        }

        [Fact]
        public void AddKudo_PublicBoardAcceptanceAllExternalUser_AddsKudosWithStatusNew()
        {
            Board board = CreateBoard(5, "boardName", "creator", new[] { "sender", "receiver" })
                            .WithPublicity(true)
                            .WithAcceptance(AcceptanceType.All);
            DbContext.Fill(new List<Board> { board });
            var kudo = CreateKudo(board, "externalUser", "receiver", false);

            var addedKudo = Manager.Add(kudo.SenderId, kudo);

            addedKudo.Status.Should().Be(KudoStatus.New);
        }

        [Fact]
        public void AddKudo_PublicBoardAcceptanceAllInternalUser_AddsKudosWithStatusNew()
        {
            Board board = CreateBoard(5, "boardName", "creator", new[] { "sender", "receiver" })
                            .WithPublicity(true)
                            .WithAcceptance(AcceptanceType.All);
            DbContext.Fill(new List<Board> { board });
            var kudo = CreateKudo(board, "sender", "receiver", false);

            var addedKudo = Manager.Add(kudo.SenderId, kudo);

            addedKudo.Status.Should().Be(KudoStatus.New);
        }

        [Fact]
        public void AddKudo_PublicBoardAcceptanceExternalOnlyExternalUser_AddsKudosWithStatusNew()
        {
            Board board = CreateBoard(5, "boardName", "creator", new[] { "sender", "receiver" })
                            .WithPublicity(true)
                            .WithAcceptance(AcceptanceType.FromExternalUsersOnly);
            DbContext.Fill(new List<Board> { board });
            var kudo = CreateKudo(board, "externalUser", "receiver", false);

            var addedKudo = Manager.Add(kudo.SenderId, kudo);

            addedKudo.Status.Should().Be(KudoStatus.New);
        }

        [Fact]
        public void AddKudo_PublicBoardAcceptanceExternalOnlyInternalUser_AddsKudosWithStatusAccepted()
        {
            Board board = CreateBoard(5, "boardName", "creator", new[] { "sender", "receiver" })
                            .WithPublicity(true)
                            .WithAcceptance(AcceptanceType.FromExternalUsersOnly);
            DbContext.Fill(new List<Board> { board });
            var kudo = CreateKudo(board, "sender", "receiver", false);

            var addedKudo = Manager.Add(kudo.SenderId, kudo);

            addedKudo.Status.Should().Be(KudoStatus.Accepted);
        }

        [Fact]
        public void AddKudo_PublicBoardAcceptanceAll_AddsNotificationToBoardOwnerAboutNewKudoToAccept()
        {
            Board board = CreateBoard(5, "boardName", "creator", new[] { "sender", "receiver" })
                            .WithPublicity(true)
                            .WithAcceptance(AcceptanceType.All);
            DbContext.Fill(new List<Board> { board });
            var kudo = CreateKudo(board, "sender", "receiver", false);

            Manager.Add(kudo.SenderId, kudo);

            Notification notification = DbContext.Notifications.FirstOrDefault(x => x.ReceiverId == "creator" && x.Type == NotificationTypes.NewKudoToAccept);
            notification.Should().NotBeNull();
        }

        [Fact]
        public void AddKudo_PublicBoardAcceptanceExternalOnlyExternalUser_AddsNotificationToBoardOwnerAboutNewKudoToAccept()
        {
            Board board = CreateBoard(5, "boardName", "creator", new[] { "sender", "receiver" })
                            .WithPublicity(true)
                            .WithAcceptance(AcceptanceType.FromExternalUsersOnly);
            DbContext.Fill(new List<Board> { board });
            var kudo = CreateKudo(board, "externalUser", "receiver", false);

            Manager.Add(kudo.SenderId, kudo);

            Notification notification = DbContext.Notifications.FirstOrDefault(x => x.ReceiverId == "creator" && x.Type == NotificationTypes.NewKudoToAccept);
            notification.Should().NotBeNull();
        }

        [Fact]
        public void AddKudo_WhenKudoNeedsToBeAccepted_KudoReceiverDoesntGetNotificationAboutNewKudo()
        {
            Board board = CreateBoard(5, "boardName", "creator", new[] { "sender", "receiver" })
                            .WithPublicity(true)
                            .WithAcceptance(AcceptanceType.All);
            DbContext.Fill(new List<Board> { board });
            var kudo = CreateKudo(board, "sender", "receiver", false);

            Manager.Add(kudo.SenderId, kudo);

            Notification notification = DbContext.Notifications.FirstOrDefault(x => x.ReceiverId == "receiver" && x.Type == NotificationTypes.KudoAdded);
            notification.Should().BeNull();
        }

        private Board CreateBoard(int boardId, string name, string creator, IEnumerable<string> users)
        {
            return new Board
            {
                Id = boardId,
                Name = name,
                UserBoards = users.Select(x => new UserBoard(x, boardId)).ToList(),
                CreatorId = creator,
            };
        }
    }
}
