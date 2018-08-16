using iKudo.Domain.Enums;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using System;

namespace iKudo.Domain.Tests.Notifications
{
    public class NotifierTestsBase : BaseTest
    {
        public NotifierTestsBase()
        {
            Notifier = new Notifier(DbContext);
        }

        public IManageNotifications Notifier { get; set; }

        protected Notification CreateNotification(string senderId, string receiverId, DateTime creationDate, NotificationTypes type)
        {
            return new Notification
            {
                SenderId = senderId,
                Sender = new User { Id = senderId },
                ReceiverId = receiverId,
                Receiver = new User { Id = receiverId },
                CreationDate = creationDate,
                Type = type
            };
        }
    }
}
