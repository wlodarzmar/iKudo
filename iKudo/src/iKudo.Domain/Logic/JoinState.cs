using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using System;

namespace iKudo.Domain.Logic
{
    public abstract class JoinState
    {
        public abstract string Name { get; }

        public abstract JoinStatus Status { get; }

        public abstract void ChangeNew(JoinRequest request, JoinState newStatus);
    }

    public class New : JoinState
    {
        public override string Name => "New";

        public override JoinStatus Status => JoinStatus.Waiting;

        public override void ChangeNew(JoinRequest request, JoinState newStatus)
        {
            request.State = newStatus;
        }
    }

    public class Accepted : JoinState
    {
        public override string Name => "Accepted";

        public override JoinStatus Status => JoinStatus.Accepted;

        public override void ChangeNew(JoinRequest request, JoinState newStatus)
        {
            throw new InvalidOperationException("Join request is already accepted");
        }
    }

    public class Rejected : JoinState
    {
        public override string Name => "Rejected";

        public override JoinStatus Status => JoinStatus.Rejected;

        public override void ChangeNew(JoinRequest request, JoinState newStatus)
        {
            throw new InvalidOperationException("Join request is already rejected");
        }
    }
}
