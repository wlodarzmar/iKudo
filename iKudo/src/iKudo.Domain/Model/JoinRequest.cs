using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace iKudo.Domain.Model
{
    public class JoinRequest
    {
        public JoinRequest()
        {
            Board = new Board();
            Status = JoinStatus.Waiting;
            BaseJoinStatus = new New(this);
        }

        public JoinRequest(int boardId, string candidateId, DateTime creationDate)
        {
            BoardId = boardId;
            CandidateId = candidateId;
            CreationDate = creationDate;
            Status = JoinStatus.Waiting;
            BaseJoinStatus = new New(this);
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
            BaseJoinStatus.Change(new Accepted(this));
        }

        public void Reject(string userPerformingActionId, DateTime decisionDate)
        {
            DecisionUserId = userPerformingActionId;
            DecisionDate = decisionDate;
            Status = JoinStatus.Rejected;
            BaseJoinStatus.Change(new Rejected(this));
        }

        public virtual BaseJoinStatus BaseJoinStatus { get; set; }
    }

    public enum JoinStatus
    {
        Accepted = 1,
        Rejected,
        Waiting,
    }

    public abstract class BaseJoinStatus
    {
        public BaseJoinStatus(JoinRequest joinRequest)
        {
            JoinRequest = joinRequest;
        }

        public JoinRequest JoinRequest { get; set; }
        public int Id { get; set; }

        public abstract void Change(BaseJoinStatus newStatus);
    }

    public class New : BaseJoinStatus
    {
        public New(JoinRequest joinRequest) : base(joinRequest)
        {
        }

        public override void Change(BaseJoinStatus newStatus)
        {
            JoinRequest.BaseJoinStatus = newStatus;
        }
    }

    public class Accepted : BaseJoinStatus
    {
        public Accepted(JoinRequest joinRequest) : base(joinRequest)
        {
        }

        public override void Change(BaseJoinStatus newStatus)
        {
            throw new InvalidOperationException("Join request is already accepted");
        }
    }

    public class Rejected : BaseJoinStatus
    {
        public Rejected(JoinRequest joinRequest) : base(joinRequest)
        {
        }

        public override void Change(BaseJoinStatus newStatus)
        {
            throw new InvalidOperationException("Join request is already rejected");
        }
    }
}
