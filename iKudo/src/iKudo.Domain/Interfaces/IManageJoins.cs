using iKudo.Domain.Model;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface IManageJoins
    {
        JoinRequest Join(int boardId, string candidateId);

        IEnumerable<JoinRequest> GetJoinRequests(string userId);

        IEnumerable<JoinRequest> GetJoinRequests(int boardId);

        JoinRequest AcceptJoin(int joinRequestId, string userIdPerformingAction);

        JoinRequest RejectJoin(int joinRequestId, string userIdPerformingAction);

    }
}
