using iKudo.Domain.Enums;
using System;

namespace iKudo.Domain.Model
{
    public class Notification
    {
        private string v1;
        private string v2;
        private DateTime now;
        private NotificationTypes boardJoinAccepted;

        public Notification()
        {
        }

        public Notification(string senderId, string receiverId, DateTime creationDate, NotificationTypes notificationType)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            CreationDate = creationDate;
            Type = notificationType;
        }

        public int Id { get; set; }

        public NotificationTypes Type { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        public DateTime CreationDate { get; set; }

        public int? BoardId { get; set; }

        public virtual Board Board { get; set; }

        public DateTime? ReadDate { get; set; }

        public bool IsRead => ReadDate.HasValue;
    }
}
