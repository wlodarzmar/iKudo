using System;

namespace iKudo.Domain.Model
{
    public class JoinRequest
    {
        public string Id { get; set; }

        public string CandidateId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? DecisionDate { get; set; }

        public bool IsAccepted { get; set; }

        public string DecisionUserId { get; set; }

        public int BoardId { get; set; }

        public Board Board { get; set; }
    }
}
