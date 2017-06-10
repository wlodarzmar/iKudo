﻿using System;
using System.Collections.Generic;
using System.Linq;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;

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
        
        IEnumerable<Notification> INotify.Get(NotificationSearchCriteria notificationSearchCriteria)
        {
            throw new NotImplementedException();
        }
    }
}
