using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface IProvideNotifications
    {
        IEnumerable<Notification> Get(NotificationSearchCriteria searchCriteria, SortCriteria sortCriteria);
    }
}