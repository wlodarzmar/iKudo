using System;

namespace iKudo.Domain.Model
{
    public class JoinRequest
    {
        public JoinRequest() { }

        public int BoardId { get; set; }

        public virtual Board Board { get; set; }

        public string CandidateId { get; set; }

        public string Id { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? DecisionDate { get; set; }

        public bool IsAccepted { get; set; }

        public string DecisionUserId { get; set; }
    }
}
