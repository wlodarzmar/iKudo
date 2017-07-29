﻿using iKudo.Domain.Enums;

namespace iKudo.Domain.Model
{
    public class Kudo
    {
        public int Id { get; set; }

        public KudoType Type { get; set; }

        public string ReceiverId { get; set; }

        public string SenderId { get; set; }

        public string Description { get; set; }

        public bool IsAnonymous { get; set; }

        public int BoardId { get; set; }
    }
}
