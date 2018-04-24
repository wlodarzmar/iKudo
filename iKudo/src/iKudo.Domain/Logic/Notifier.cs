using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace iKudo.Domain.Logic
{
    public class Notifier : INotify
    {
        private KudoDbContext dbContext;

        public Notifier(KudoDbContext dbContext)
        {
            this.dbContext = dbContext;
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
            IQueryable<Notification> notifications = dbContext.Notifications.Include(x => x.Board)
                                                                            .Include(x => x.Sender);

            if (!string.IsNullOrWhiteSpace(searchCriteria?.Receiver))
            {
                notifications = notifications.Where(x => x.ReceiverId == searchCriteria.Receiver);
            }
            if (searchCriteria?.IsRead.HasValue == true)
            {
                notifications = notifications.Where(x => x.IsRead == searchCriteria.IsRead);
            }

            if (!string.IsNullOrWhiteSpace(sortCriteria?.RawCriteria))
            {
                notifications = notifications.OrderBy(sortCriteria.Criteria);
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
