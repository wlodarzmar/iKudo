using System;
using System.Collections.Generic;
using System.Linq;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;

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

        IEnumerable<Notification> INotify.Get(NotificationSearchCriteria criteria)
        {
            IQueryable<Notification> notifications = dbContext.Notifications;

            if (!string.IsNullOrWhiteSpace(criteria.Receiver))
            {
                notifications = notifications.Where(x => x.ReceiverId == criteria.Receiver);
            }
            if (criteria.IsRead.HasValue)
            {
                notifications = notifications.Where(x => x.IsRead == criteria.IsRead);
            }

            return notifications.ToList();
        }
    }
}
