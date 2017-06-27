using System;
using System.Collections.Generic;
using System.Linq;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic;

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

        public void Update(string userPerformingActionId, Notification notification)
        {
            if (notification.ReceiverId != userPerformingActionId)
            {
                throw new UnauthorizedAccessException("You don't have access to this object");
            }

            dbContext.Update(notification);
            dbContext.SaveChanges();
        }

        IEnumerable<NotificationMessage> INotify.Get(NotificationSearchCriteria searchCriteria, SortCriteria sortCriteria)
        {
            IQueryable<Notification> notifications = dbContext.Notifications.Include(x => x.Board);

            if (!string.IsNullOrWhiteSpace(searchCriteria.Receiver))
            {
                notifications = notifications.Where(x => x.ReceiverId == searchCriteria.Receiver);
            }
            if (searchCriteria.IsRead.HasValue)
            {
                notifications = notifications.Where(x => x.IsRead == searchCriteria.IsRead);
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
