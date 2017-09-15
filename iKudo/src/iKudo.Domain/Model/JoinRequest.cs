using iKudo.Domain.Logic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iKudo.Domain.Model
{
    public class JoinRequest
    {
        public JoinRequest()
        {
            Board = new Board();
            BaseJoinStatus = JoinStateFactory.GetState(JoinStatus.Waiting);
        }

        public JoinRequest(int boardId, string candidateId, DateTime creationDate)
        {
            BoardId = boardId;
            CandidateId = candidateId;
            CreationDate = creationDate;
            BaseJoinStatus = JoinStateFactory.GetState(JoinStatus.Waiting);
        }

        public int Id { get; set; }

        public int BoardId { get; set; }

        public virtual Board Board { get; set; }

        public string CandidateId { get; set; }

        public DateTime CreationDate { get; private set; }

        public DateTime? DecisionDate { get; private set; }

        public string DecisionUserId { get; private set; }

        [Obsolete]
        public JoinStatus Status { get; private set; }

        public void Accept(string userPerformingActionId, DateTime decisionDate)
        {
            DecisionUserId = userPerformingActionId;
            DecisionDate = decisionDate;
            BaseJoinStatus.ChangeNew(this, JoinStateFactory.GetState(JoinStatus.Accepted));
        }

        public void Reject(string userPerformingActionId, DateTime decisionDate)
        {
            DecisionUserId = userPerformingActionId;
            DecisionDate = decisionDate;
            BaseJoinStatus.ChangeNew(this, JoinStateFactory.GetState(JoinStatus.Rejected));
        }

        public virtual BaseJoinStatus BaseJoinStatus { get; set; }

        [Required]
        public string JoinStatusName => BaseJoinStatus.Name;
    }

    public enum JoinStatus
    {
        [Display(Name = "Accepted")]
        Accepted = 1,

        [Display(Name = "Rejected")]
        Rejected,

        [Display(Name = "New")]
        Waiting,
    }

    public abstract class BaseJoinStatus
    {
        public abstract string Name { get; }

        public abstract JoinStatus Status { get; }

        public abstract void ChangeNew(JoinRequest request, BaseJoinStatus newStatus);
    }

    public class New : BaseJoinStatus
    {
        public override string Name => "New";

        public override JoinStatus Status => JoinStatus.Waiting;

        public override void ChangeNew(JoinRequest request, BaseJoinStatus newStatus)
        {
            request.BaseJoinStatus = newStatus;
        }
    }

    public class Accepted : BaseJoinStatus
    {
        public override string Name => "Accepted";

        public override JoinStatus Status => JoinStatus.Accepted;

        public override void ChangeNew(JoinRequest request, BaseJoinStatus newStatus)
        {
            throw new InvalidOperationException("Join request is already accepted");
        }
    }

    public class Rejected : BaseJoinStatus
    {
        public override string Name => "Rejected";

        public override JoinStatus Status => JoinStatus.Rejected;

        public override void ChangeNew(JoinRequest request, BaseJoinStatus newStatus)
        {
            throw new InvalidOperationException("Join request is already rejected");
        }
    }
}
