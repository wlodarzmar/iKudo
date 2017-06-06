using System;

namespace iKudo.Domain.Model
{
    public class JoinRequest
    {
        public JoinRequest()
        {
            Board = new Board();
            Status = JoinStatus.Waiting;
        }

        public JoinRequest(int boardId, string candidateId, DateTime creationDate)
        {
            BoardId = boardId;
            CandidateId = candidateId;
            CreationDate = creationDate;
            Status = JoinStatus.Waiting;
        }

        public int Id { get; set; }

        public int BoardId { get; set; }

        public virtual Board Board { get; set; }

        public string CandidateId { get; set; }

        public DateTime CreationDate { get; private set; }

        public DateTime? DecisionDate { get; private set; }
        
        public string DecisionUserId { get; private set; }

        public JoinStatus Status { get; private set; }

        public void Accept(string userPerformingActionId, DateTime decisionDate)
        {
            DecisionUserId = userPerformingActionId;
            DecisionDate = decisionDate;
            Status = JoinStatus.Accepted;
        }

        public void Reject(string userPerformingActionId, DateTime decisionDate)
        {
            DecisionUserId = userPerformingActionId;
            DecisionDate = decisionDate;
            Status = JoinStatus.Rejected;
        }
    }

    public enum JoinStatus
    {
        Accepted = 1,
        Rejected,
        Waiting,
    }
}
