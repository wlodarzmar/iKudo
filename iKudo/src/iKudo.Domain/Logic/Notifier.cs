using System;
using System.Collections.Generic;
using System.Linq;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace iKudo.Domain.Logic
{
    public class Notifier : INotify
    {
        private KudoDbContext dbContext;

        public Notifier(KudoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public int Count(string receiverId)
        {
            return dbContext.Notifications.Count(x => x.ReceiverId == receiverId);
        }

        IEnumerable<NotificationMessage> INotify.Get(NotificationSearchCriteria criteria)
        {
            IQueryable<Notification> notifications = dbContext.Notifications.Include(x => x.Board);

            if (!string.IsNullOrWhiteSpace(criteria.Receiver))
            {
                notifications = notifications.Where(x => x.ReceiverId == criteria.Receiver);
            }
            if (criteria.IsRead.HasValue)
            {
                notifications = notifications.Where(x => x.IsRead == criteria.IsRead);
            }

            List<NotificationMessage> notificationMessages = new List<NotificationMessage>();
            foreach (var notification in notifications)
            {
                notificationMessages.Add(new NotificationMessage(notification));
            }

            return notificationMessages;
        }
    }
}
