using iKudo.Domain.Model;
using System;

namespace iKudo.Domain.Criteria
{
    public class JoinSearchCriteria
    {
        public int? BoardId { get; set; }

        public string CandidateId { get; set; }

        public JoinStatus? Status { get; set; }

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
                    JoinStatus joinStatus;
                    if (Enum.TryParse(value, true, out joinStatus))
                    {
                        Status = joinStatus;
                    }
                }
            }
        }
    }
}
