﻿using iKudo.Domain.Model;
using System;

namespace iKudo.Dtos
{
    public class JoinDTO
    {
        public int Id { get; set; }

        public int BoardId { get; set; }

        public string CandidateId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? DecisionDate { get; set; }

        public bool? IsAccepted { get; set; }

        public string DecisionUserId { get; set; }

        public string CandidateName { get; set; }

        public string CandidateEmail { get; set; }

        public JoinStatus Status { get; set; }
    }
}
