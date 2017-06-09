using iKudo.Domain.Enums;
using System;

namespace iKudo.Dtos
{
    public class NotificationDTO
    {
        public int Id { get; set; }

        public NotificationTypes Type { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        public DateTime CreationDate { get; set; }

        public int? BoardId { get; set; }

        public DateTime? ReadDate { get; set; }

        public bool IsRead => ReadDate.HasValue;
    }
}
