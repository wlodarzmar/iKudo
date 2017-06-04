using iKudo.Domain.Model;
using System;

namespace iKudo.Domain.Criteria
{
    public class JoinSearchCriteria
    {
        public JoinSearchCriteria()
        {
        }

        public JoinSearchCriteria(int boardId, string status)
        {
            BoardId = boardId;
            if (!string.IsNullOrWhiteSpace(status))
            {
                JoinStatus joinStatus;
                if (Enum.TryParse(status, true, out joinStatus))
                {
                    Status = joinStatus;
                }
            }
        }

        public int BoardId { get; set; }

        public JoinStatus? Status { get; set; }
    }
}
