using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface INotify
    {
        IEnumerable<NotificationMessage> Get(NotificationSearchCriteria notificationSearchCriteria, SortCriteria sortCriteria);
        void Update(string userPerformingActionId, Notification notification);
    }
}
