using iKudo.Domain.Model;

namespace iKudo.Domain.Interfaces
{
    public interface IManageNotifications
    {
        //IEnumerable<Notification> Get(NotificationSearchCriteria searchCriteria, SortCriteria sortCriteria);
        void Update(string userPerformingActionId, Notification notification);
    }
}
