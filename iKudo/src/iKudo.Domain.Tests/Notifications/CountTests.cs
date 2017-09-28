using FluentAssertions;
using iKudo.Domain.Enums;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using System;
using System.Collections.Generic;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class CountTests : BaseTest
    {
        [Fact]
        public void Count_WithoutNotificationsForReceiver_ReturnsZero()
        {
            List<Notification> existingNotifications = new List<Notification> {
                new Notification("sender", "receiver", DateTime.Now, NotificationTypes.BoardJoinAccepted)
            };
            DbContext.Fill(existingNotifications);
            INotify notifier = new Notifier(DbContext);

            int notificationCount = notifier.Count("someReceiver");

            notificationCount.Should().Be(0);
        }

        [Fact]
        public void Count_WithNotificationForReceiver_ReturnsValidCount()
        {
            List<Notification> existingNotifications = new List<Notification> {
                new Notification("sender", "receiver1", DateTime.Now, NotificationTypes.BoardJoinAccepted),
                new Notification("sender", "receiver2", DateTime.Now, NotificationTypes.BoardJoinAccepted)
            };
            DbContext.Fill(existingNotifications);
            INotify notifier = new Notifier(DbContext);

            int notificationCount = notifier.Count("receiver2");

            notificationCount.Should().Be(1);
        }
    }
}
