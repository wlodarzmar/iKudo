using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using System;

namespace iKudo.Domain.Logic
{
    public class NotificationManager : IManageNotifications
    {
        private readonly KudoDbContext dbContext;

        public NotificationManager(KudoDbContext dbContext)
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
    }
}
