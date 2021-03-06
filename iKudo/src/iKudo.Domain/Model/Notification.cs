﻿using iKudo.Domain.Enums;
using System;

namespace iKudo.Domain.Model
{
    public class Notification
    {
        public Notification()
        {
        }

        public int Id { get; set; }

        public NotificationTypes Type { get; set; }

        public string SenderId { get; set; }

        public virtual User Sender { get; set; }

        public string ReceiverId { get; set; }

        public virtual User Receiver { get; set; }

        public DateTime CreationDate { get; set; }

        public int? BoardId { get; set; }

        public virtual Board Board { get; set; }

        public DateTime? ReadDate { get; set; }

        public bool IsRead => ReadDate.HasValue;

        public int? KudoId { get; set; }

        public virtual Kudo Kudo { get; set; }
    }
}
