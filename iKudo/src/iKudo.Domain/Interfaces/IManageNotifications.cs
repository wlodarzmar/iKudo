using iKudo.Domain.Model;

namespace iKudo.Domain.Interfaces
{
    public interface IManageNotifications
    {
        void Update(string userPerformingActionId, Notification notification);
    }
}
