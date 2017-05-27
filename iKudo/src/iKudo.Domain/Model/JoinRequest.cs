using System;

namespace iKudo.Domain.Model
{
    public class JoinRequest
    {
        public JoinRequest()
        {
            Board = new Board();
        }

        public JoinRequest(int boardId, string candidateId) : this()
        {
            BoardId = boardId;
            CandidateId = candidateId;
        }

        public int Id { get; set; }

        public int BoardId { get; set; }

        public virtual Board Board { get; set; }

        public string CandidateId { get; set; }
        
        public DateTime CreationDate { get; set; }

        public DateTime? DecisionDate { get; private set; }

        public bool? IsAccepted { get; private set; }

        public string DecisionUserId { get; private set; }

        public void Accept(string userPerformingActionId, DateTime decisionDate)
        {
            DecisionUserId = userPerformingActionId;
            DecisionDate = decisionDate;
            IsAccepted = true;
        }

        public void Reject(string userPerformingActionId, DateTime decisionDate)
        {
            DecisionUserId = userPerformingActionId;
            DecisionDate = decisionDate;
            IsAccepted = false;
        }
    }
}
