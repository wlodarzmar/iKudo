﻿using iKudo.Domain.Enums;
using System;
using System.Collections.Generic;

namespace iKudo.Domain.Model
{
    public class Board
    {
        public Board()
        {
            JoinRequests = new List<JoinRequest>();
            UserBoards = new List<UserBoard>();
        }

        public int Id { get; set; }

        public string CreatorId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? ModificationDate { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsPublic => !IsPrivate;

        public AcceptanceType AcceptanceType { get; set; }

        public virtual ICollection<JoinRequest> JoinRequests { get; set; }

        public virtual ICollection<UserBoard> UserBoards { get; set; }

        public virtual ICollection<Kudo> Kudos { get; internal set; }

        internal void Add(DateTime creationDate)
        {
            CreationDate = creationDate;
            IsPrivate = true;
            UserBoards.Add(new UserBoard(CreatorId, Id));
        }

        internal void Update(DateTime modificationDate)
        {
            ModificationDate = modificationDate;
        }
    }
}
