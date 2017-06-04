using iKudo.Domain.Model;
using System.Collections.Generic;
using iKudo.Domain.Criteria;

namespace iKudo.Domain.Interfaces
{
    public interface IManageJoins
    {
        JoinRequest Join(int boardId, string candidateId);

        IEnumerable<JoinRequest> GetJoins(JoinSearchCriteria criteria);

        JoinRequest AcceptJoin(int joinRequestId, string userIdPerformingAction);

        JoinRequest RejectJoin(int joinRequestId, string userIdPerformingAction);
    }
}
