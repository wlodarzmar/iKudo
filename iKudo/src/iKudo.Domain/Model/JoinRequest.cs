using System;

namespace iKudo.Domain.Model
{
    // class NewJoinRequest ??
    public class NewJoinRequest
    {
        public NewJoinRequest() { }

        public NewJoinRequest(int boardId, string candidateId)
        {
            BoardId = boardId;
            CandidateId = candidateId;
        }

        public int BoardId { get; set; }

        public Board Board { get; set; }

        public string CandidateId { get; set; }
    }

    public class JoinRequest : NewJoinRequest
    {
        public JoinRequest() { }

        //public JoinRequest(int boardId, string candidateId)
        //{
        //    BoardId = boardId;
        //    CandidateId = candidateId;
        //}

        public string Id { get; set; }
        
        public DateTime CreationDate { get; set; }

        public DateTime? DecisionDate { get; set; }

        public bool IsAccepted { get; set; }

        public string DecisionUserId { get; set; }
    }
}
