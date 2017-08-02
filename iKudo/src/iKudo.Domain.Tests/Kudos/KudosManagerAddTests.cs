using FluentAssertions;
using iKudo.Domain.Enums;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Kudos
{
    public class KudosManagerAddKudoTests : BaseTest
    {
        Mock<IProvideTime> timeProviderMock;
        Board existingBoard = new Board
        {
            Id = 1,
            Name = "board",
            UserBoards = new List<UserBoard> { new UserBoard("sender", 1), new UserBoard("receiver", 1) }
        };

        public KudosManagerAddKudoTests()
        {
            timeProviderMock = new Mock<IProvideTime>();
            DbContext.Fill(new List<Board> { existingBoard });
        }

        [Fact]
        public void AddKudo_ValidKudo_KudoAdded()
        {
            IManageKudos manager = new KudosManager(DbContext, timeProviderMock.Object);
            Kudo kudo = new Kudo
            {
                Board = existingBoard,
                BoardId = existingBoard.Id,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = true,
                ReceiverId = "receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob
            };
            kudo = manager.Add(kudo.SenderId, kudo);

            DbContext.Kudos.FirstOrDefault(x => x.Id == kudo.Id).Should().NotBeNull();
        }

        [Fact]
        public void AddKudo_UserTriesToAddKudoToForeignBoard_ThrowsUnauthorizedAccessException()
        {
            IManageKudos manager = new KudosManager(DbContext, timeProviderMock.Object);
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

            manager.Invoking(x => x.Add(kudo.SenderId, kudo)).ShouldThrow<UnauthorizedAccessException>();
        }

        [Fact]
        public void AddKudo_UserTriesToAddKudoForUserThatIsNotMemberOfBoard_ThrowsInvalidOperationException()
        {
            IManageKudos manager = new KudosManager(DbContext, timeProviderMock.Object);
            Kudo kudo = new Kudo
            {
                BoardId = existingBoard.Id,
                Board = existingBoard,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = true,
                ReceiverId = "some other receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob
            };

            manager.Invoking(x => x.Add(kudo.SenderId, kudo)).ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void AddKudo_ValidKudo_AddsNotificationAboutNewKudo()
        {
            IManageKudos manager = new KudosManager(DbContext, timeProviderMock.Object);
            Kudo kudo = new Kudo
            {
                Board = existingBoard,
                BoardId = existingBoard.Id,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = true,
                ReceiverId = "receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob
            };
            kudo = manager.Add(kudo.SenderId, kudo);

            DbContext.Notifications.Any(x => x.BoardId == existingBoard.Id
                                        && x.ReceiverId == kudo.ReceiverId
                                        && x.SenderId == kudo.SenderId
                                        && x.Type == NotificationTypes.KudoAdded)
                                   .Should().BeTrue();
        }

        [Fact]
        public void AddKudo_UserPerformingActionDifferentThanKudoSender_ThrowsForbidden()
        {
            IManageKudos manager = new KudosManager(DbContext, timeProviderMock.Object);
            Kudo kudo = new Kudo
            {
                Board = existingBoard,
                BoardId = existingBoard.Id,
                CreationDate = DateTime.Now,
                Description = "desc",
                IsAnonymous = true,
                ReceiverId = "receiver",
                SenderId = "sender",
                Type = KudoType.GoodJob
            };
            manager.Invoking(x => x.Add("otherUser", kudo)).ShouldThrow<UnauthorizedAccessException>();
        }
    }
}
