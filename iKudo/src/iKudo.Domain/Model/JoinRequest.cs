using iKudo.Domain.Enums;
using iKudo.Domain.Logic;
using System;
using System.ComponentModel.DataAnnotations;

namespace iKudo.Domain.Model
{
    public class JoinRequest
    {
        public JoinRequest()
        {
            Board = new Board();
            State = JoinStateFactory.GetState(JoinStatus.Waiting);
        }

        public JoinRequest(int boardId, string candidateId, DateTime creationDate) : this()
        {
            Board = new Board { Id = boardId };
            CandidateId = candidateId;
            CreationDate = creationDate;
        }

        public int Id { get; set; }

        public int BoardId { get; set; }

        public virtual Board Board { get; set; }

        public string CandidateId { get; set; }

        public virtual User Candidate { get; set; }

        public DateTime CreationDate { get; private set; }

        public DateTime? DecisionDate { get; private set; }

        public string DecisionUserId { get; private set; }

        public virtual JoinState State { get; set; }

        [Required]
        public string StateName
        {
            get { return State.Name; }
            protected set
            {
                State = JoinStateFactory.GetState(value);
            }
        }

        public void Accept(string userPerformingActionId, DateTime decisionDate)
        {
            DecisionUserId = userPerformingActionId;
            DecisionDate = decisionDate;
            State.ChangeNew(this, JoinStateFactory.GetState(JoinStatus.Accepted));
        }

        public void Reject(string userPerformingActionId, DateTime decisionDate)
        {
            DecisionUserId = userPerformingActionId;
            DecisionDate = decisionDate;
            State.ChangeNew(this, JoinStateFactory.GetState(JoinStatus.Rejected));
        }
    }
}
