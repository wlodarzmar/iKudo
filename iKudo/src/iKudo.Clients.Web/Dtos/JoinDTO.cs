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
    }
}
