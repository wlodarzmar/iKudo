namespace iKudo.Dtos
{
    public class JoinDecision
    {
        public JoinDecision(int joinRequestId, bool isAccepted)
        {
            JoinRequestId = joinRequestId;
            IsAccepted = isAccepted;
        }

        public int JoinRequestId { get; set; }

        public bool IsAccepted { get; set; }
    }
}
