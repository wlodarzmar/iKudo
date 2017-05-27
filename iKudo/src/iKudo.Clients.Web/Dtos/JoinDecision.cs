namespace iKudo.Dtos
{
    public class JoinDecision
    {
        public JoinDecision(string joinRequestId, bool isAccepted)
        {
            JoinRequestId = joinRequestId;
            IsAccepted = isAccepted;
        }

        public string JoinRequestId { get; set; }

        public bool IsAccepted { get; set; }
    }
}
