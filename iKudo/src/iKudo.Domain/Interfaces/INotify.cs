using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface INotify
    {
        int Count(string receiverId);
        IEnumerable<NotificationMessage> Get(NotificationSearchCriteria notificationSearchCriteria);
        void Update(string userPerformingActionId, Notification notification);
    }
}
