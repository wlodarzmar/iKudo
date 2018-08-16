using iKudo.Domain.Criteria;
using iKudo.Domain.Extensions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace iKudo.Domain.Logic
{
    public class NotificationProvider : IProvideNotifications
    {
        private readonly KudoDbContext dbContext;

        public NotificationProvider(KudoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<Notification> Get(NotificationSearchCriteria searchCriteria, SortCriteria sortCriteria)
        {
            IQueryable<Notification> notifications = dbContext.Notifications.Include(x => x.Board)
                                                                            .Include(x => x.Sender);

            notifications = notifications.WhereIf(x => x.ReceiverId == searchCriteria.Receiver, !string.IsNullOrWhiteSpace(searchCriteria?.Receiver));
            notifications = notifications.WhereIf(x => x.IsRead == searchCriteria.IsRead, searchCriteria?.IsRead.HasValue == true);
            notifications = notifications.OrderByIf(sortCriteria?.Criteria, !string.IsNullOrWhiteSpace(sortCriteria?.RawCriteria));

            return notifications;
        }
    }
}
