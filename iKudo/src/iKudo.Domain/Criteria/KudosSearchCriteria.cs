using System;

namespace iKudo.Domain.Criteria
{
    public class KudosSearchCriteria
    {
        public string UserPerformingActionId { get; set; }

        public int? BoardId { get; set; }

        public string User { get; set; }

        public UserSearchTypes UserSearchType { get; set; }
    }

    public enum UserSearchTypes
    {
        Both,
        SenderOnly,
        ReceiverOnly
    }
}
