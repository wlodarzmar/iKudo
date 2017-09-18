using iKudo.Domain.Enums;
using System;

namespace iKudo.Domain.Criteria
{
    public class JoinSearchCriteria
    {
        public int? BoardId { get; set; }

        public string CandidateId { get; set; }

        public JoinStatus? Status { get; set; }

        public string StatusName { get; set; }

        public string StatusText
        {
            get
            {
                return Status.ToString();
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (Enum.TryParse(value, true, out JoinStatus joinStatus))
                    {
                        Status = joinStatus;
                    }
                }
            }
        }
    }
}
