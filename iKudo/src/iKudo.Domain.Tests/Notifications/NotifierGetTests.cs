using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Notifications
{
    public class NotifierGetTests : BaseTest
    {
        [Fact]
        public void Get_WithSender_ReturnsValidCollection()
        {
            ICollection<Notification> existingNotifications = new List<Notification> {
                new Notification("sender", "receiver1", DateTime.Now, NotificationTypes.BoardJoinAccepted),
                new Notification("sender", "receiver2", DateTime.Now, NotificationTypes.BoardJoinAccepted),
            };
            DbContext.Fill(existingNotifications);
            INotify notifier = new Notifier(DbContext);

            IEnumerable<Notification> result = notifier.Get(new Criteria.NotificationSearchCriteria { Receiver = "receiver1" }, It.IsAny<SortCriteria>());

            result.Count().Should().Be(1);
        }

        [Fact]
        public void Get_WithIsRead_ReturnsValidCollection()
        {
            ICollection<Notification> existingNotifications = new List<Notification> {
                new Notification("sender", "receiver1", DateTime.Now, NotificationTypes.BoardJoinAccepted) {ReadDate = DateTime.Now },
                new Notification("sender", "receiver2", DateTime.Now, NotificationTypes.BoardJoinAccepted),
            };
            DbContext.Fill(existingNotifications);
            INotify notifier = new Notifier(DbContext);

            IEnumerable<Notification> result = notifier.Get(new NotificationSearchCriteria { IsRead = true }, It.IsAny<SortCriteria>());

            result.Count().Should().Be(1);
        }

        [Fact]
        public void Get_WithSortOnType_ReturnsOrderedCollection()
        {
            ICollection<Notification> existingNotifications = new List<Notification> {
                new Notification("sender", "receiver2", DateTime.Now, NotificationTypes.BoardJoinRejected),
                new Notification("sender", "receiver1", DateTime.Now, NotificationTypes.BoardJoinAccepted),
            };

            DbContext.Fill(existingNotifications);
            INotify notifier = new Notifier(DbContext);

            List<NotificationMessage> result = notifier.Get(It.IsAny<NotificationSearchCriteria>(), new SortCriteria { RawCriteria = "type" }).ToList();

            result[0].Type.Should().Be(NotificationTypes.BoardJoinAccepted);
            result[1].Type.Should().Be(NotificationTypes.BoardJoinRejected);
        }

        [Fact]
        public void Get_WithSort_ReturnsOrderedCollection()
        {
            DateTime date1 = DateTime.Now;
            DateTime date2 = date1.AddMinutes(1);

            ICollection<Notification> existingNotifications = new List<Notification> {
                new Notification("sender", "receiver2", date1, NotificationTypes.BoardJoinRejected),
                new Notification("sender", "receiver1", date2, NotificationTypes.BoardJoinAccepted),
            };

            DbContext.Fill(existingNotifications);
            INotify notifier = new Notifier(DbContext);

            List<NotificationMessage> result = notifier.Get(It.IsAny<NotificationSearchCriteria>(), new SortCriteria("-creationDate")).ToList();

            result[0].CreationDate.Should().Be(date2);
            result[1].CreationDate.Should().Be(date1);
        }
    }
}
