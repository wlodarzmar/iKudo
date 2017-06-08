using iKudo.Domain.Enums;
using System;

namespace iKudo.Domain.Model
{
    public class Notification
    {
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
