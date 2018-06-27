using iKudo.Domain.Criteria;
using iKudo.Domain.Extensions;
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
        private readonly KudoDbContext dbContext;

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

        IEnumerable<Notification> INotify.Get(NotificationSearchCriteria searchCriteria, SortCriteria sortCriteria)
        {
            IQueryable<Notification> notifications = dbContext.Notifications.Include(x => x.Board)
                                                                            .Include(x => x.Sender);

            notifications = notifications.WhereIf(x => x.ReceiverId == searchCriteria.Receiver, !string.IsNullOrWhiteSpace(searchCriteria?.Receiver));
            notifications = notifications.WhereIf(x => x.IsRead == searchCriteria.IsRead, searchCriteria?.IsRead.HasValue == true);

            if (!string.IsNullOrWhiteSpace(sortCriteria?.RawCriteria))
            {
                notifications = notifications.OrderBy(sortCriteria.Criteria);
            }

            return notifications;
        }
    }
}
