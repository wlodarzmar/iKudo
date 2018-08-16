using FluentAssertions;
using iKudo.Domain.Enums;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace iKudo.Domain.Tests.Notifications
{
    public class UpdateTests : BaseTest
    {
        [Fact]
        public void Update_UpdatesNotification()
        {
            Notification notification = new Notification { ReadDate = null, Id = 1, Board = new Board { Id = 1, Name = "name" }, BoardId = 1, ReceiverId = "receiver", SenderId = "sen", Type = NotificationTypes.BoardJoinAccepted };
            List<Notification> existingNotifications = new List<Notification> {
                notification
            };
            DbContext.Fill(existingNotifications);
            IManageNotifications notifier = new Notifier(DbContext);

            notification.ReadDate = DateTime.Now;

            notifier.Update("receiver", notification);

            DbContext.Notifications.Any(x => x.Id == 1 && x.IsRead).Should().Be(true);
        }

        [Fact]
        public void Update_UserTriesReadForeignNotification_ThrowsUnauthorizedAccessException()
        {
            Notification notification = new Notification { ReadDate = null, Id = 1, Board = new Board { Id = 1, Name = "name" }, BoardId = 1, ReceiverId = "receiver", SenderId = "sen", Type = NotificationTypes.BoardJoinAccepted };

            IManageNotifications notifier = new Notifier(DbContext);

            notification.ReadDate = DateTime.Now;

            notifier.Invoking(x => x.Update("otherUser", notification)).ShouldThrow<UnauthorizedAccessException>();
        }
    }
}
