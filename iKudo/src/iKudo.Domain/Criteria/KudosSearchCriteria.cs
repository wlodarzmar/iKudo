using iKudo.Domain.Enums;

namespace iKudo.Domain.Criteria
{
    public class KudosSearchCriteria
    {
        public KudosSearchCriteria()
        {
            Statuses = new KudoStatus[] { };
        }

        public string UserPerformingActionId { get; set; }

        public int? BoardId { get; set; }

        public string User { get; set; }

        public UserSearchTypes UserSearchType { get; set; }

        //public KudoStatus Status { get; set; }
        public KudoStatus[] Statuses { get; set; }
    }

    public enum UserSearchTypes
    {
        Both,
        SenderOnly,
        ReceiverOnly
    }
}
