using iKudo.Domain.Model;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface IManageJoins
    {
        JoinRequest Join(int boardId, string candidateId);

        ICollection<JoinRequest> GetJoinRequests(string userId);

        JoinRequest AcceptJoin(string joinRequestId, string userIdPerformingAction);

        JoinRequest RejectJoin(string joinRequestId, string userIdPerformingAction);
    }
}
